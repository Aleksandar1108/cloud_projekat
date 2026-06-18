namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IConsumptionLimitNotifiedRepository
    {
        Task<bool> TryMarkNotifiedAsync(Guid smartMeterId, int year, int month, CancellationToken ct = default);
    }
}
