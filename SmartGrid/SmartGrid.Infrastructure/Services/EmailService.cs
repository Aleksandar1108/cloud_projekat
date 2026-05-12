using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using SmartGrid.Application.Interfaces;
using SmartGrid.Infrastructure.Common;

namespace SmartGrid.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string link)
        {
            await SendMailjetAsync(to, subject, $"Activate your SmartGrid account by visiting this link: {link}", EmailTemplate.Activation(link));
        }

        public async Task SendHtmlNotificationAsync(string to, string subject, string htmlBody)
        {
            var plain = System.Text.RegularExpressions.Regex.Replace(htmlBody, "<.*?>", string.Empty);
            await SendMailjetAsync(to, subject, plain, htmlBody);
        }

        private static async Task SendMailjetAsync(string to, string subject, string textPart, string htmlPart)
        {
            MailjetClient client = new MailjetClient(
                Environment.GetEnvironmentVariable("MAILJET_API_KEY"),
                Environment.GetEnvironmentVariable("MAILJET_API_SECRET")
            );

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, Environment.GetEnvironmentVariable("MAILJET_SENDER_EMAIL"))
            .Property(Send.FromName, Environment.GetEnvironmentVariable("MAILJET_SENDER_NAME"))
            .Property(Send.Subject, subject)
            .Property(Send.TextPart, textPart)
            .Property(Send.HtmlPart, htmlPart)
            .Property(Send.Recipients, new JArray {
            new JObject {
                { "Email", to }
            }
            });

            MailjetResponse response = await client.PostAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                Console.WriteLine($"ErrorInfo: {response.GetErrorInfo()}");
                Console.WriteLine($"ErrorMessage: {response.GetErrorMessage()}");
            }
        }
    }
}
