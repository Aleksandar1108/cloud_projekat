using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(UserId id, CancellationToken ct);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken ct);

        Task AddAsync(User user, CancellationToken ct);
        Task UpdateAsync(User user, CancellationToken ct);
        Task DeleteAsync(UserId id, CancellationToken ct);
    }
}
