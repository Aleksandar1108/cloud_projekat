using SmartGrid.Application.Features.ManualReadings;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IManualReadingRepository
    {
        Task<ManualReadingDto> CreateAsync(ManualReadingDto reading, CancellationToken ct = default);
        Task<ManualReadingDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyCollection<ManualReadingDto>> GetAllAsync(ManualReadingStatus? status = null, CancellationToken ct = default);
        Task<bool> MarkProcessedAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyCollection<ManualReadingDto>> GetProcessedByPeriodAsync(DateTime periodStartUtc, DateTime periodEndUtc, CancellationToken ct = default);
    }
}
