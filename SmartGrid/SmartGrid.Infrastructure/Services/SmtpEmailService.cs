using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces;
using SmartGrid.Infrastructure.Common.Options;
using System.Net;
using System.Net.Mail;

namespace SmartGrid.Infrastructure.Services
{
    internal class SmtpEmailService(
        IOptions<SmtpOptions> options,
        ILogger<SmtpEmailService> logger) : IEmailService
    {
        private readonly SmtpOptions _options = options.Value;
        private readonly ILogger<SmtpEmailService> _logger = logger;

        public async Task SendAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken ct = default)
        {
            var targets = recipients
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (targets.Count == 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.FromEmail))
            {
                _logger.LogWarning("SMTP is not configured. Skipping monthly billing email sending.");
                return;
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_options.FromEmail, _options.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            foreach (var target in targets)
            {
                message.To.Add(target);
            }

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl
            };

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                client.Credentials = new NetworkCredential(_options.Username, _options.Password);
            }

            ct.ThrowIfCancellationRequested();
            await client.SendMailAsync(message, ct);
        }
    }
}
