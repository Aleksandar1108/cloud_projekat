using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(UserId id);
        Task<User?> GetByEmailAsync(string email);
        Task<IReadOnlyCollection<User>> GetAllAsync();

        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(UserId id);
    }
}
