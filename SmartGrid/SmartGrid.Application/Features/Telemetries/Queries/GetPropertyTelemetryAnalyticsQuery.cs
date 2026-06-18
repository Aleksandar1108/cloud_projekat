using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using System.Reflection;
using Azure;

namespace SmartGrid.Application.Features.Telemetries.Queries
{
    public record DailyTariffConsumptionDto(
        DateTime DayUtc,
        double HigherTariffKwh,
        double LowerTariffKwh
    );

    public record TelemetryTrendPointDto(
        DateTime TimestampUtc,
        double LoadPercentage,
        double CurrentPowerKw,
        double? Voltage
    );

    public record TelemetryStatusCardDto(
        string CurrentTariff,
        double CurrentPowerKw,
        double LoadPercentage,
        double? CurrentVoltage,
        bool IsOnline,
        DateTime? LastHeartbeatUtc
    );

    public record SmartMeterTelemetryAnalyticsDto(
        Guid SmartMeterId,
        string Label,
        string? DeviceUuid,
        IReadOnlyCollection<DailyTariffConsumptionDto> DailyConsumption,
        IReadOnlyCollection<TelemetryTrendPointDto> Trend,
        TelemetryStatusCardDto Status
    );

    public record PropertyTelemetryAnalyticsDto(
        Guid PropertyId,
        DateTime GeneratedAtUtc,
        IReadOnlyCollection<SmartMeterTelemetryAnalyticsDto> Meters
    );

    public record GetPropertyTelemetryAnalyticsQuery(Guid PropertyId, Guid UserId) : IRequest<Result<PropertyTelemetryAnalyticsDto>>;

    internal sealed class GetPropertyTelemetryAnalyticsHandler(
        IPropertyRepository propertyRepository,
        ISmartMeterRepository smartMeterRepository,
        ITelemetryRepository telemetryRepository,
        IDeviceStatusQueryRepository deviceStatusQueryRepository)
        : IRequestHandler<GetPropertyTelemetryAnalyticsQuery, Result<PropertyTelemetryAnalyticsDto>>
    {
        private const double SampleIntervalHours = 0.5;
        private const int DailyWindowDays = 7;

        public async Task<Result<PropertyTelemetryAnalyticsDto>> Handle(GetPropertyTelemetryAnalyticsQuery request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
                return Result<PropertyTelemetryAnalyticsDto>.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result<PropertyTelemetryAnalyticsDto>.Failure("Access denied.", ErrorType.Unauthorized);

            var meters = await smartMeterRepository.GetByPropertyIdAsync(request.PropertyId, ct);

            var utcToday = DateTime.UtcNow.Date;
            var consumptionStart = utcToday.AddDays(-(DailyWindowDays - 1));
            var consumptionEnd = utcToday.AddDays(1);
            var trendStart = DateTime.UtcNow.AddHours(-24);

            var telemetry = await telemetryRepository.GetByPeriodAsync(consumptionStart, consumptionEnd, ct);
            IReadOnlyCollection<Domain.Models.DeviceStatus> statuses;
            try
            {
                statuses = await deviceStatusQueryRepository.GetAllAsync(ct);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // DeviceStatuses table can be missing in early environments.
                statuses = Array.Empty<Domain.Models.DeviceStatus>();
            }
            catch
            {
                // If statuses source is temporarily unavailable, keep analytics endpoint responsive.
                statuses = Array.Empty<Domain.Models.DeviceStatus>();
            }

            var statusByDeviceId = statuses
                .Where(s => !string.IsNullOrWhiteSpace(s.DeviceId.Value))
                .GroupBy(s => s.DeviceId.Value, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.LastHeartbeat).First(),
                    StringComparer.OrdinalIgnoreCase);

            var meterDtos = new List<SmartMeterTelemetryAnalyticsDto>(meters.Count);
            foreach (var meter in meters)
            {
                var deviceUuid = meter.DeviceUUID;
                var meterTelemetry = string.IsNullOrWhiteSpace(deviceUuid)
                    ? new List<Domain.Models.Telemetry>()
                    : telemetry
                        .Where(t => string.Equals(t.DeviceId.Value, deviceUuid, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(t => t.Timestamp)
                        .ToList();

                var daily = BuildDailyConsumption(meterTelemetry, consumptionStart, DailyWindowDays);
                var trend = meterTelemetry
                    .Where(t => t.Timestamp >= trendStart)
                    .Select(t => new TelemetryTrendPointDto(
                        t.Timestamp,
                        Math.Round(t.LoadPercentage.Value, 2),
                        Math.Round(t.CurrentPower.Value, 2),
                        TryExtractVoltage(t)))
                    .ToList();

                var latest = meterTelemetry.LastOrDefault();
                statusByDeviceId.TryGetValue(deviceUuid ?? string.Empty, out var status);

                var statusCard = new TelemetryStatusCardDto(
                    CurrentTariff: ResolveTariffLabel(latest?.Timestamp),
                    CurrentPowerKw: Math.Round(status?.CurrentPower.Value ?? latest?.CurrentPower.Value ?? 0, 2),
                    LoadPercentage: Math.Round(status?.LoadPercentage.Value ?? latest?.LoadPercentage.Value ?? 0, 2),
                    CurrentVoltage: status is null ? TryExtractVoltage(latest) : TryExtractVoltage(status),
                    IsOnline: status?.IsOnline(DateTime.UtcNow) ?? false,
                    LastHeartbeatUtc: status?.LastHeartbeat ?? latest?.Timestamp);

                meterDtos.Add(new SmartMeterTelemetryAnalyticsDto(
                    meter.Id,
                    meter.Label,
                    deviceUuid,
                    daily,
                    trend,
                    statusCard));
            }

            return Result<PropertyTelemetryAnalyticsDto>.Success(new PropertyTelemetryAnalyticsDto(
                request.PropertyId,
                DateTime.UtcNow,
                meterDtos));
        }

        private static IReadOnlyCollection<DailyTariffConsumptionDto> BuildDailyConsumption(
            IReadOnlyCollection<Domain.Models.Telemetry> telemetry,
            DateTime startDayUtc,
            int days)
        {
            var buckets = Enumerable.Range(0, days)
                .Select(offset => startDayUtc.AddDays(offset))
                .ToDictionary(day => day, _ => (Higher: 0.0, Lower: 0.0));

            foreach (var sample in telemetry)
            {
                var day = sample.Timestamp.Date;
                if (!buckets.ContainsKey(day))
                    continue;

                var kwh = sample.CurrentPower.Value * SampleIntervalHours;
                var existing = buckets[day];
                if (sample.Timestamp.Hour >= 7 && sample.Timestamp.Hour < 23)
                {
                    buckets[day] = (existing.Higher + kwh, existing.Lower);
                }
                else
                {
                    buckets[day] = (existing.Higher, existing.Lower + kwh);
                }
            }

            return buckets
                .OrderBy(x => x.Key)
                .Select(x => new DailyTariffConsumptionDto(
                    x.Key,
                    Math.Round(x.Value.Higher, 2),
                    Math.Round(x.Value.Lower, 2)))
                .ToList();
        }

        private static string ResolveTariffLabel(DateTime? timestampUtc)
        {
            if (timestampUtc is null)
                return "N/A";

            return timestampUtc.Value.Hour >= 7 && timestampUtc.Value.Hour < 23 ? "VT" : "NT";
        }

        private static double? TryExtractVoltage(object? source)
        {
            if (source is null)
                return null;

            var propertyNames = new[] { "Voltage", "CurrentVoltage", "GridVoltage" };
            var sourceType = source.GetType();
            foreach (var propertyName in propertyNames)
            {
                var property = sourceType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property is null)
                    continue;

                var value = property.GetValue(source);
                if (value is double number && !double.IsNaN(number) && !double.IsInfinity(number))
                    return number;

                if (value is float floatNumber && !float.IsNaN(floatNumber) && !float.IsInfinity(floatNumber))
                    return floatNumber;

                if (value is decimal decimalNumber)
                    return (double)decimalNumber;
            }

            return null;
        }
    }
}
