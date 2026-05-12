namespace SmartGrid.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string link);

        /// <summary>Generic HTML notification (alerts, limits).</summary>
        Task SendHtmlNotificationAsync(string to, string subject, string htmlBody);
    }
}
