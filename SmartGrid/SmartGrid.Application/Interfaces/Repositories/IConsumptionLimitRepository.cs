namespace SmartGrid.Application.Interfaces.Repositories;

public sealed class ConsumptionLimitSetting
{
    public required Guid UserId { get; init; }
    public required Guid DeviceId { get; init; }
    public required decimal LimitKwh { get; init; }
    public decimal? LimitRsd { get; init; }
}

public interface IConsumptionLimitRepository
{
    Task<ConsumptionLimitSetting?> GetByDeviceIdAsync(Guid deviceId, CancellationToken ct = default);

    Task<IReadOnlyList<ConsumptionLimitSetting>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default);

    Task UpsertAsync(ConsumptionLimitSetting row, CancellationToken ct = default);

    Task<bool> WasLimitEmailSentAsync(Guid userId, Guid deviceId, int yearMonth, CancellationToken ct = default);

    Task RecordLimitEmailSentAsync(Guid userId, Guid deviceId, int yearMonth, CancellationToken ct = default);
}
