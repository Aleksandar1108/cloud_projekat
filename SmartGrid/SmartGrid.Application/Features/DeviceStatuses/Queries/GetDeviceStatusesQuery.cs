using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common.Options;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Constants;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.DeviceStatuses.Queries
{
    // QUERY DTO
    public record DeviceStatusDto(
        string DeviceId,
        DeviceType DeviceType,
        double CurrentPower,
        double LoadPercentage,
        bool IsOnline,
        bool IsUnderperforming,
        bool IsOverloaded,
        string CurrentFirmwareVersion,
        string? TargetFirmwareVersion,
        UpdateStatus UpdateStatus,
        string? Label = null
    );

    public record GetDeviceStatusesQuery() : IRequest<Result<IEnumerable<DeviceStatusDto>>?>;

    // HANDLER
    internal class GetDeviceStatusesHandler(
        IDeviceStatusQueryRepository deviceStatusQueryRepository,
        ISmartMeterRepository smartMeterRepository,
        ITelemetryRepository telemetryRepository,
        IMapper<DeviceStatus, DeviceStatusDto> mapper,
        IDateTimeProvider dateTimeProvider,
        IOptions<AlertNotificationsOptions> alertOptions,
        ILogger<GetDeviceStatusesHandler> logger
    ) : IRequestHandler<GetDeviceStatusesQuery, Result<IEnumerable<DeviceStatusDto>>?>
    {
        public async Task<Result<IEnumerable<DeviceStatusDto>>?> Handle(GetDeviceStatusesQuery request, CancellationToken ct)
        {
            try
            {
                var now = dateTimeProvider.UtcNow;
                var offlineThreshold = TimeSpan.FromMinutes(alertOptions.Value.OfflineThresholdMinutes);

                var statuses = await deviceStatusQueryRepository.GetAllAsync(ct);
                var result = statuses.Select(mapper.Map).ToList();
                var knownDeviceIds = result
                    .Select(x => x.DeviceId)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var pairedMeters = await smartMeterRepository.GetAllAsync(ct);
                foreach (var meter in pairedMeters)
                {
                    var deviceKey = !string.IsNullOrWhiteSpace(meter.DeviceUUID)
                        ? meter.DeviceUUID
                        : meter.SerialNumber ?? meter.Id.ToString();

                    if (knownDeviceIds.Contains(deviceKey))
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(meter.DeviceUUID))
                    {
                        var latest = await telemetryRepository.GetLatestByDeviceIdAsync(meter.DeviceUUID, ct);
                        if (latest is not null)
                        {
                            var isOnline = now - latest.Timestamp <= offlineThreshold;
                            var load = latest.LoadPercentage.Value;
                            result.Add(new DeviceStatusDto(
                                meter.DeviceUUID,
                                latest.DeviceType,
                                latest.CurrentPower.Value,
                                load,
                                isOnline,
                                load < DeviceStatusLimits.UnderperformingLoad,
                                load > DeviceStatusLimits.OverloadedLoad,
                                latest.FirmwareVersion.Value,
                                null,
                                UpdateStatus.UpToDate,
                                meter.Label));
                            knownDeviceIds.Add(meter.DeviceUUID);
                            continue;
                        }
                    }

                    result.Add(new DeviceStatusDto(
                        deviceKey,
                        DeviceType.Unknown,
                        0,
                        0,
                        false,
                        false,
                        false,
                        "-",
                        null,
                        UpdateStatus.UpToDate,
                        meter.Label));

                    knownDeviceIds.Add(deviceKey);
                }

                return Result<IEnumerable<DeviceStatusDto>>.Success(
                    result.OrderByDescending(x => x.IsOnline).ThenBy(x => x.Label ?? x.DeviceId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while retreiving devices..");
                return Result<IEnumerable<DeviceStatusDto>>.Failure("Failed to retrieve devices.",
                 ErrorType.Failure);
            }
        }
    }
}
