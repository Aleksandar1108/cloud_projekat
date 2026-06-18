using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface ITelemetryRepository
    {
        Task SaveAsync(Telemetry telemetry, CancellationToken ct = default);
        Task<IReadOnlyCollection<Telemetry>> GetByPeriodAsync(
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            CancellationToken ct = default);
        Task<IReadOnlyCollection<Telemetry>> GetByDeviceAndPeriodAsync(
            string deviceId,
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            CancellationToken ct = default);
        Task<Telemetry?> GetLatestByDeviceIdAsync(string deviceId, CancellationToken ct = default);
    }
}
