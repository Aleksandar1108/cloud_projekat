namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IAlertDispatchStateRepository
    {
        Task<bool> ShouldNotifyAsync(string deviceId, string alertKind, TimeSpan cooldown, CancellationToken ct = default);
        Task ClearAsync(string deviceId, string alertKind, CancellationToken ct = default);
    }
}
