using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IEmailActivationRepository
    {
        Task<EmailActivation?> GetByTokenAsync(string token);
        Task AddAsync(EmailActivation activation);
        Task DeleteAsync(EmailActivation activation);
    }
}
