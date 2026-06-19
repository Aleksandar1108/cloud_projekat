using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces;
using SmartGrid.Infrastructure.Common;
using SmartGrid.Infrastructure.Common.Options;
using System.Net;
using System.Net.Mail;

namespace SmartGrid.Infrastructure.Services
{
    public class EmailService(ILogger<EmailService> logger, IOptions<SmtpOptions> smtpOptions) : IEmailService
    {
        private readonly SmtpOptions _smtp = smtpOptions.Value;

        public Task SendActivationEmailAsync(string to, string subject, string link, CancellationToken ct)
        {
            var html = EmailTemplate.Activation(link);
            var text = $"Activate your SmartGrid account by visiting this link: {link}";
            return SendAsync(to, subject, text, html, ct);
        }

        public Task SendPasswordResetEmailAsync(string to, string subject, string link, CancellationToken ct)
        {
            var html = EmailTemplate.PasswordReset(link);
            var text = $"Reset your SmartGrid password by visiting this link: {link}";
            return SendAsync(to, subject, text, html, ct);
        }

        public Task SendEmailAsync(string to, string subject, string textBody, string htmlBody, CancellationToken ct)
            => SendAsync(to, subject, textBody, htmlBody, ct);

        public Task SendEmailAsync(string to, string subject, string text, CancellationToken ct)
            => SendAsync(to, subject, text, htmlBody: null, ct);

        private async Task SendAsync(string to, string subject, string textBody, string? htmlBody, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_smtp.Host)
                || string.IsNullOrWhiteSpace(_smtp.Username)
                || string.IsNullOrWhiteSpace(_smtp.Password)
                || string.IsNullOrWhiteSpace(_smtp.FromEmail))
            {
                logger.LogWarning(
                    "SMTP is not configured; skipping email send. To={To}, Subject={Subject}",
                    to,
                    subject);
                return;
            }

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_smtp.FromEmail, _smtp.FromName),
                    Subject = subject,
                    Body = htmlBody ?? textBody,
                    IsBodyHtml = htmlBody is not null,
                };
                message.To.Add(to);

                using var client = new SmtpClient(_smtp.Host, _smtp.Port)
                {
                    EnableSsl = _smtp.EnableSsl,
                    Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                };

                await client.SendMailAsync(message, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {To} with subject {Subject}", to, subject);
            }
        }
    }
}
