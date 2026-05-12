using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface ISmartMeterRepository
    {
        Task<IReadOnlyCollection<SmartMeter>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);
        Task<SmartMeter?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<SmartMeter?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default);
        Task AddAsync(SmartMeter smartMeter, CancellationToken ct = default);
        Task UpdateAsync(SmartMeter smartMeter, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
