namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IStripeEventRepository
    {
        Task<bool> TryMarkProcessedAsync(string eventId, CancellationToken ct = default);
    }
}

