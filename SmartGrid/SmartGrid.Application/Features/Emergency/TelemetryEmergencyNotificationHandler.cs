using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common.Options;
using SmartGrid.Application.Features.Telemetries.Events;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Application.Features.Emergency;

internal sealed class TelemetryEmergencyNotificationHandler(
    IAlertQueueService alertQueueService,
    IOptions<AlertNotificationOptions> alertOptions,
    ITelemetryRepository telemetryRepository,
    IConsumptionLimitRepository consumptionLimitRepository,
    IUserRepository userRepository,
    IEmailService emailService,
    ILogger<TelemetryEmergencyNotificationHandler> logger)
    : INotificationHandler<TelemetryProcessedEvent>
{
    public async Task Handle(TelemetryProcessedEvent notification, CancellationToken ct)
    {
        var telemetry = notification.Telemetry;
        var opts = alertOptions.Value;

        try
        {
            var minV = telemetry.GetMinimumReportedVoltageVolts();
            if (minV is double v && v < opts.CriticalVoltageThresholdVolts)
            {
                var alertResult = Alert.Create(
                    telemetry.DeviceId.Value,
                    AlertType.Critical,
                    $"Napon ispod praga ({opts.CriticalVoltageThresholdVolts} V): min {v:F1} V.");

                if (alertResult.IsSuccess)
                {
                    await alertQueueService.SendAlertAsync(alertResult.Value, ct);
                    logger.LogWarning("[EMERGENCY] Low voltage alert enqueued for device {DeviceId}", telemetry.DeviceId);
                }
            }

            if (!telemetry.EnergyDeltaKwh.HasValue)
                return;

            if (!Guid.TryParse(telemetry.DeviceId.Value, out var deviceGuid))
                return;

            var limit = await consumptionLimitRepository.GetByDeviceIdAsync(deviceGuid, ct);
            if (limit is null || limit.LimitKwh <= 0)
                return;

            var utc = DateTime.UtcNow;
            var yearMonth = utc.Year * 100 + utc.Month;
            var monthSum = await telemetryRepository.GetEnergyDeltaKwhSumForDeviceUtcMonthAsync(
                telemetry.DeviceId.Value,
                utc.Year,
                utc.Month,
                ct);

            if ((decimal)monthSum <= limit.LimitKwh)
                return;

            if (await consumptionLimitRepository.WasLimitEmailSentAsync(limit.UserId, deviceGuid, yearMonth, ct))
                return;

            var user = await userRepository.GetByIdAsync(UserId.FromGuid(limit.UserId));
            if (user is null)
                return;

            const string subject = "[SmartGrid] Prekoračenje mesečnog limita potrošnje";
            var body =
                "<!DOCTYPE html><html><body style='font-family:Arial,sans-serif'>" +
                "<p>Detektovano je prekoračenje limita za Vaše brojilo (merna tačka).</p>" +
                "<p><b>Uređaj:</b> " + System.Net.WebUtility.HtmlEncode(telemetry.DeviceId.Value) + "<br/>" +
                $"<b>Ukupno u tekućem mesecu (kWh):</b> {monthSum:F2}<br/>" +
                $"<b>Vaš limit (kWh):</b> {limit.LimitKwh}</p>" +
                "<p>Ovo obaveštenje se šalje jednom mesečno pri prvom prekoračenju (zahtev specifikacije).</p>" +
                "</body></html>";

            await emailService.SendHtmlNotificationAsync(user.Email.Value, subject, body);
            await consumptionLimitRepository.RecordLimitEmailSentAsync(limit.UserId, deviceGuid, yearMonth, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[EMERGENCY] Failed processing emergency rules for device {DeviceId}", telemetry.DeviceId);
        }
    }
}
