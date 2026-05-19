using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IEmailActivationRepository
    {
        Task<EmailActivation?> GetByTokenAsync(string token, CancellationToken ct);
        Task AddAsync(EmailActivation activation, CancellationToken ct);
        Task DeleteAsync(EmailActivation activation, CancellationToken ct);
    }
}
