using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common.Options;
using SmartGrid.Application.Features.Alerts.Commands;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Features.Alerts.Commands
{
    public record CheckSmartMeterOfflineCommand : IRequest<Result>;

    internal class CheckSmartMeterOfflineHandler(
        ISmartMeterRepository smartMeterRepository,
        ITelemetryRepository telemetryRepository,
        IAlertDispatchStateRepository alertDispatchStateRepository,
        IAlertQueueService alertQueueService,
        IOptions<AlertNotificationsOptions> alertOptions,
        ILogger<CheckSmartMeterOfflineHandler> logger) : IRequestHandler<CheckSmartMeterOfflineCommand, Result>
    {
        private const string OfflineAlertKind = "DeviceOffline";

        public async Task<Result> Handle(CheckSmartMeterOfflineCommand request, CancellationToken ct)
        {
            var options = alertOptions.Value;
            var offlineThreshold = TimeSpan.FromMinutes(options.OfflineThresholdMinutes);
            var now = DateTime.UtcNow;
            var meters = await smartMeterRepository.GetAllPairedAsync(ct);

            foreach (var meter in meters)
            {
                if (string.IsNullOrWhiteSpace(meter.DeviceUUID))
                {
                    continue;
                }

                var latest = await telemetryRepository.GetLatestByDeviceIdAsync(meter.DeviceUUID, ct);
                var isOffline = latest is null || now - latest.Timestamp > offlineThreshold;
                if (!isOffline)
                {
                    await alertDispatchStateRepository.ClearAsync(meter.DeviceUUID, OfflineAlertKind, ct);
                    continue;
                }

                if (!await alertDispatchStateRepository.ShouldNotifyAsync(
                        meter.DeviceUUID,
                        OfflineAlertKind,
                        TimeSpan.FromDays(365),
                        ct))
                {
                    continue;
                }

                var lastSeen = latest?.Timestamp.ToString("u") ?? "nikada";
                var message =
                    $"Pametno brojilo '{meter.Label}' ({meter.DeviceUUID}) ne salje podatke. Poslednje merenje: {lastSeen} UTC.";

                var alertResult = Alert.Create(meter.DeviceUUID, AlertType.Critical, message);
                if (alertResult.IsFailure)
                {
                    logger.LogWarning(
                        "[ALERT] Failed to create offline alert for meter {MeterId}: {Error}",
                        meter.Id,
                        alertResult.Error?.Message);
                    continue;
                }

                await alertQueueService.SendAlertAsync(alertResult.Value, ct);
            }

            return Result.Success();
        }
    }
}
