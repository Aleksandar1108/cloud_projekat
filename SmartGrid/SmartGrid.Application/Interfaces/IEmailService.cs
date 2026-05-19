namespace SmartGrid.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendActivationEmailAsync(string to, string subject, string link, CancellationToken ct);
        Task SendEmailAsync(string to, string subject, string textBody,string htmlBody, CancellationToken ct);

        Task SendEmailAsync(string to, string subject, string text, CancellationToken ct);

        Task SendPasswordResetEmailAsync(string to, string subject, string link, CancellationToken ct);
    }
}
