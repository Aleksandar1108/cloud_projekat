using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Features.ConsumptionLimits.Queries;

public sealed record ConsumptionLimitSummaryDto(
    Guid DeviceId,
    string DeviceName,
    decimal LimitKwh,
    double MonthConsumptionKwh,
    bool LimitExceeded,
    bool NotificationSentThisMonth);

public record GetConsumptionLimitSummariesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<ConsumptionLimitSummaryDto>>>;

internal sealed class GetConsumptionLimitSummariesHandler(
    IConsumptionLimitRepository consumptionLimitRepository,
    ITelemetryRepository telemetryRepository,
    IDeviceRepository deviceRepository,
    ILogger<GetConsumptionLimitSummariesHandler> logger)
    : IRequestHandler<GetConsumptionLimitSummariesQuery, Result<IReadOnlyList<ConsumptionLimitSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<ConsumptionLimitSummaryDto>>> Handle(
        GetConsumptionLimitSummariesQuery request,
        CancellationToken ct)
    {
        try
        {
            var limits = await consumptionLimitRepository.GetAllByUserIdAsync(request.UserId, ct);
            if (limits.Count == 0)
                return Result<IReadOnlyList<ConsumptionLimitSummaryDto>>.Success(Array.Empty<ConsumptionLimitSummaryDto>());

            var utc = DateTime.UtcNow;
            var yearMonth = utc.Year * 100 + utc.Month;
            var list = new List<ConsumptionLimitSummaryDto>(limits.Count);

            foreach (var limit in limits)
            {
                var deviceIdStr = EntityId.Create(limit.DeviceId.ToString());
                if (deviceIdStr.IsFailure)
                    continue;

                var device = await deviceRepository.GetByIdAnyTypeAsync(deviceIdStr.Value, ct);
                var deviceName = device?.Name ?? limit.DeviceId.ToString("D");

                var monthSum = await telemetryRepository.GetEnergyDeltaKwhSumForDeviceUtcMonthAsync(
                    limit.DeviceId.ToString(),
                    utc.Year,
                    utc.Month,
                    ct);

                var exceeded = limit.LimitKwh > 0 && (decimal)monthSum > limit.LimitKwh;
                var notified = await consumptionLimitRepository.WasLimitEmailSentAsync(
                    limit.UserId,
                    limit.DeviceId,
                    yearMonth,
                    ct);

                list.Add(new ConsumptionLimitSummaryDto(
                    limit.DeviceId,
                    deviceName,
                    limit.LimitKwh,
                    monthSum,
                    exceeded,
                    notified));
            }

            return Result<IReadOnlyList<ConsumptionLimitSummaryDto>>.Success(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load consumption limit summaries for user {UserId}.", request.UserId);
            return Result<IReadOnlyList<ConsumptionLimitSummaryDto>>.Failure(
                "Failed to load consumption limit summaries.",
                ErrorType.Failure);
        }
    }
}
