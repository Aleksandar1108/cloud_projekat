using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common.Options;
using SmartGrid.Application.Interfaces;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Features.Alerts.Commands
{
    // COMMAND
    public record ProcessAlertCommand(Alert Alert) : IRequest<Result>;

    // HANDLER
    internal class ProcessAlertHandler(
        ILogger<ProcessAlertHandler> logger,
        IEmailService emailService,
        IOptions<AlertNotificationOptions> alertOptions)
        : IRequestHandler<ProcessAlertCommand, Result>
    {
        public async Task<Result> Handle(ProcessAlertCommand request, CancellationToken ct)
        {
            var alert = request.Alert;
            string logMsg = $"[ALARM] {alert.AlertType} on {alert.DeviceId}: {alert.Message}";
            if (alert.AlertType == AlertType.Critical)
                logger.LogError(logMsg);
            else
                logger.LogWarning(logMsg);

            if (alert.AlertType != AlertType.Critical)
                return Result.Success();

            var raw = alertOptions.Value.NetworkAdminEmails;
            if (string.IsNullOrWhiteSpace(raw))
                return Result.Success();

            var recipients = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (recipients.Length == 0)
                return Result.Success();

            var html =
                "<!DOCTYPE html><html><body style='font-family:Arial,sans-serif'>" +
                "<h2>SmartGrid — hitno upozorenje</h2>" +
                $"<p><b>Tip:</b> {alert.AlertType}<br/>" +
                $"<b>Uređaj:</b> {alert.DeviceId.Value}<br/>" +
                $"<b>Poruka:</b> {System.Net.WebUtility.HtmlEncode(alert.Message.Value)}<br/>" +
                $"<b>Vreme (UTC):</b> {alert.Timestamp:O}</p>" +
                "</body></html>";

            foreach (var to in recipients)
            {
                try
                {
                    await emailService.SendHtmlNotificationAsync(to, "[SmartGrid] Kritično upozorenje", html);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send critical alert email to {Recipient}", to);
                }
            }

            return Result.Success();
        }
    }
}