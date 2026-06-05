using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common.Options;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Features.Alerts.Commands
{
    // COMMAND
    public record ProcessAlertCommand(Alert Alert) : IRequest<Result>;

    // HANDLER
    internal class ProcessAlertHandler(
        IEmailService emailService,
        IUserRepository userRepository,
        IOptions<AlertNotificationsOptions> alertOptions,
        ILogger<ProcessAlertHandler> logger) : IRequestHandler<ProcessAlertCommand, Result>
    {
        public async Task<Result> Handle(ProcessAlertCommand request, CancellationToken ct)
        {
            var alert = request.Alert;
            string logMsg = $"[ALARM] {alert.AlertType} on {alert.DeviceId}: {alert.Message}";

            if (alert.AlertType == AlertType.Critical)
            {
                logger.LogError(logMsg);
            }
            else
            {
                logger.LogWarning(logMsg);
            }

            await SendNetworkAdminEmailsAsync(alert, ct);

            return Result.Success();
        }

        private async Task SendNetworkAdminEmailsAsync(Alert alert, CancellationToken ct)
        {
            var recipients = await ResolveNetworkAdminRecipientsAsync(ct);
            if (recipients.Count == 0)
            {
                logger.LogWarning("[ALERT] No network admin recipients configured for alert on device {DeviceId}.", alert.DeviceId);
                return;
            }

            var subject = alert.AlertType == AlertType.Critical
                ? "SmartGrid - Kriticno upozorenje mreze"
                : "SmartGrid - Upozorenje mreze";

            var body =
                $"Postovani,{Environment.NewLine}{Environment.NewLine}" +
                $"Tip: {alert.AlertType}{Environment.NewLine}" +
                $"Uredjaj: {alert.DeviceId}{Environment.NewLine}" +
                $"Vreme: {alert.Timestamp:u}{Environment.NewLine}" +
                $"Poruka: {alert.Message}{Environment.NewLine}{Environment.NewLine}" +
                "SmartGrid";

            foreach (var recipient in recipients)
            {
                await emailService.SendEmailAsync(recipient, subject, body, ct);
            }
        }

        private async Task<List<string>> ResolveNetworkAdminRecipientsAsync(CancellationToken ct)
        {
            var recipients = alertOptions.Value.NetworkAdminEmails
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var adminUsers = await userRepository.GetAllAsync(ct);
            recipients.AddRange(
                adminUsers
                    .Where(u => u.Role == UserRole.Admin)
                    .Select(u => u.Email.Value));

            return recipients
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
