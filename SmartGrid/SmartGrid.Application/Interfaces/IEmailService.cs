namespace SmartGrid.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken ct = default);
    }
}
