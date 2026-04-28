using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using SmartGrid.Application.Interfaces;
using SmartGrid.Domain.ValueObjects.User;
using SmartGrid.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartGrid.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string link)
        {
            var htmlBody = EmailTemplate.Activation(link);

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
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                Console.WriteLine($"ErrorInfo: {response.GetErrorInfo()}");
                Console.WriteLine($"ErrorMessage: {response.GetErrorMessage()}");
            }
        }
    }
}
