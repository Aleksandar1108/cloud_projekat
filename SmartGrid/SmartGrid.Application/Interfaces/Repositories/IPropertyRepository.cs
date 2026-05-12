using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IPropertyRepository
    {
        Task<IReadOnlyCollection<Property>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<Property?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Property property, CancellationToken ct = default);
        Task UpdateAsync(Property property, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
