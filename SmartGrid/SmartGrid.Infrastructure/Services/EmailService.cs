using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Infrastructure.Common;

namespace SmartGrid.Infrastructure.Services
{
    public class EmailService(ILogger<EmailService> logger) : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string link,CancellationToken ct)
        {
            var apiKey = Environment.GetEnvironmentVariable("MAILJET_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("MAILJET_API_SECRET");
            var senderEmail = Environment.GetEnvironmentVariable("MAILJET_SENDER_EMAIL");
            var senderName = Environment.GetEnvironmentVariable("MAILJET_SENDER_NAME");

            // In local/dev setups these env vars are often missing; don't fail critical flows (registration, billing).
            if (string.IsNullOrWhiteSpace(apiKey)
                || string.IsNullOrWhiteSpace(apiSecret)
                || string.IsNullOrWhiteSpace(senderEmail)
                || string.IsNullOrWhiteSpace(senderName))
            {
                logger.LogDebug(
                    "Mailjet env vars are not configured; skipping email send. To={To}, Subject={Subject}",
                    to,
                    subject);
                return;
            }

            var htmlBody = EmailTemplate.Activation(link);

            MailjetClient client = new MailjetClient(
                apiKey,
                apiSecret
            );

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, Environment.GetEnvironmentVariable("MAILJET_SENDER_EMAIL"))
            .Property(Send.FromName, senderName)
            .Property(Send.Subject, subject)
            .Property(Send.TextPart, $"Activate your SmartGrid account by visiting this link: {link}")
            .Property(Send.HtmlPart, htmlBody)
            .Property(Send.Recipients, new JArray {
            new JObject {
                { "Email", to }
            }
            });

            MailjetResponse response = await client.PostAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Mailjet send failed. StatusCode={StatusCode}, Error={ErrorMessage}",
                    response.StatusCode,
                    response.GetErrorMessage());
            }
        }
    }
}
