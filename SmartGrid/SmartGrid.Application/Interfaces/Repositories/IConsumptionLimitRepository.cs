using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public record ConsumptionLimitSettings(
        Guid UserId,
        Guid SmartMeterId,
        ConsumptionLimitUnit Unit,
        double LimitValue,
        DateTime UpdatedAtUtc);

    public interface IConsumptionLimitRepository
    {
        Task<ConsumptionLimitSettings?> GetAsync(Guid userId, Guid smartMeterId, CancellationToken ct = default);
        Task SaveAsync(ConsumptionLimitSettings settings, CancellationToken ct = default);
        Task DeleteAsync(Guid userId, Guid smartMeterId, CancellationToken ct = default);
    }
}
