using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common;
using SmartGrid.Application.Common.Options;
using SmartGrid.Application.Features.Telemetries.Events;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Application.Features.Alerts.EventHandlers
{
    internal class AnalyzeTelemetryAlertsHandler(
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        ITelemetryRepository telemetryRepository,
        ITariffModelRepository tariffModelRepository,
        IConsumptionLimitRepository consumptionLimitRepository,
        IConsumptionLimitNotifiedRepository consumptionLimitNotifiedRepository,
        IAlertDispatchStateRepository alertDispatchStateRepository,
        IAlertQueueService alertQueueService,
        IEmailService emailService,
        IOptions<AlertNotificationsOptions> alertOptions,
        ILogger<AnalyzeTelemetryAlertsHandler> logger) : INotificationHandler<TelemetryProcessedEvent>
    {
        private const string VoltageAlertKind = "VoltageCritical";
        private const string OfflineAlertKind = "DeviceOffline";

        public async Task Handle(TelemetryProcessedEvent notification, CancellationToken ct)
        {
            var telemetry = notification.Telemetry;
            var deviceId = telemetry.DeviceId.Value;
            var options = alertOptions.Value;

            await alertDispatchStateRepository.ClearAsync(deviceId, OfflineAlertKind, ct);

            if (telemetry.Voltage is double voltage && voltage < options.CriticalVoltageThresholdVolts)
            {
                await HandleVoltageAlertAsync(deviceId, voltage, options, ct);
            }
            else if (telemetry.Voltage is double healthyVoltage && healthyVoltage >= options.CriticalVoltageThresholdVolts)
            {
                await alertDispatchStateRepository.ClearAsync(deviceId, VoltageAlertKind, ct);
            }

            var smartMeter = await smartMeterRepository.GetByDeviceUUIDAsync(deviceId, ct);
            if (smartMeter is null)
            {
                return;
            }

            var property = await propertyRepository.GetByIdAsync(smartMeter.PropertyId, ct);
            if (property is null)
            {
                return;
            }

            await CheckConsumptionLimitAsync(smartMeter, property.UserId, telemetry.Timestamp, ct);
        }

        private async Task HandleVoltageAlertAsync(
            string deviceId,
            double voltage,
            AlertNotificationsOptions options,
            CancellationToken ct)
        {
            var cooldown = TimeSpan.FromMinutes(options.VoltageAlertCooldownMinutes);
            if (!await alertDispatchStateRepository.ShouldNotifyAsync(deviceId, VoltageAlertKind, cooldown, ct))
            {
                return;
            }

            var message =
                $"Kritican pad napona detektovan na uredjaju {deviceId}. Trenutni napon: {voltage:F1}V (prag: {options.CriticalVoltageThresholdVolts:F1}V).";

            await QueueCriticalAlertAsync(deviceId, message, ct);
        }

        private async Task QueueCriticalAlertAsync(string deviceId, string message, CancellationToken ct)
        {
            var alertResult = Alert.Create(deviceId, AlertType.Critical, message);
            if (alertResult.IsFailure)
            {
                logger.LogWarning("[ALERT] Failed to create alert for device {DeviceId}: {Error}", deviceId, alertResult.Error?.Message);
                return;
            }

            await alertQueueService.SendAlertAsync(alertResult.Value, ct);
        }

        private async Task CheckConsumptionLimitAsync(
            Domain.Models.SmartMeter smartMeter,
            Guid userId,
            DateTime sampleTimestamp,
            CancellationToken ct)
        {
            var limit = await consumptionLimitRepository.GetAsync(userId, smartMeter.Id, ct);
            if (limit is null || limit.LimitValue <= 0)
            {
                return;
            }

            var monthStart = new DateTime(sampleTimestamp.Year, sampleTimestamp.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);

            if (string.IsNullOrWhiteSpace(smartMeter.DeviceUUID))
            {
                return;
            }

            var samples = await telemetryRepository.GetByDeviceAndPeriodAsync(
                smartMeter.DeviceUUID,
                monthStart,
                monthEnd,
                ct);

            var monthKwh = ConsumptionCalculator.CalculateKwh(samples);
            var exceeded = false;
            string body;

            if (limit.Unit == ConsumptionLimitUnit.Kwh)
            {
                exceeded = monthKwh >= limit.LimitValue;
                body =
                    $"Postovani,{Environment.NewLine}{Environment.NewLine}" +
                    $"Detektovano je prekoracenje limita potrosnje za brojilo '{smartMeter.Label}'.{Environment.NewLine}" +
                    $"Limit: {limit.LimitValue:F2} kWh{Environment.NewLine}" +
                    $"Trenutna potrosnja u mesecu: {monthKwh:F2} kWh{Environment.NewLine}{Environment.NewLine}" +
                    "SmartGrid";
            }
            else
            {
                var tariff = await tariffModelRepository.GetActiveAsync(ct);
                if (tariff is null)
                {
                    logger.LogWarning("[ALERT] Active tariff model missing; cannot evaluate RSD consumption limit.");
                    return;
                }

                var estimatedCost = ConsumptionCalculator.EstimateEnergyCost(monthKwh, tariff);
                exceeded = estimatedCost >= limit.LimitValue;
                body =
                    $"Postovani,{Environment.NewLine}{Environment.NewLine}" +
                    $"Detektovano je prekoracenje limita potrosnje za brojilo '{smartMeter.Label}'.{Environment.NewLine}" +
                    $"Limit: {limit.LimitValue:F2} RSD{Environment.NewLine}" +
                    $"Procenjeni trosak u mesecu: {estimatedCost:F2} RSD{Environment.NewLine}{Environment.NewLine}" +
                    "SmartGrid";
            }

            if (!exceeded)
            {
                return;
            }

            if (!await consumptionLimitNotifiedRepository.TryMarkNotifiedAsync(
                    smartMeter.Id,
                    sampleTimestamp.Year,
                    sampleTimestamp.Month,
                    ct))
            {
                return;
            }

            var user = await userRepository.GetByIdAsync(UserId.FromGuid(userId), ct);
            if (user is null)
            {
                return;
            }

            await emailService.SendEmailAsync(
                user.Email.Value,
                "SmartGrid - Prekoracen limit potrosnje",
                body,
                ct);

            logger.LogInformation(
                "[ALERT] Consumption limit notification sent to user {UserId} for smart meter {SmartMeterId}.",
                userId,
                smartMeter.Id);
        }
    }
}
