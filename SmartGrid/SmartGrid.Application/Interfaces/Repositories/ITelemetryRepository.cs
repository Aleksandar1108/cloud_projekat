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

        /// <summary>Sum of EnergyDeltaKwh for a device in the given UTC calendar month (including rows without delta as 0).</summary>
        Task<double> GetEnergyDeltaKwhSumForDeviceUtcMonthAsync(
            string deviceId,
            int year,
            int month,
            CancellationToken ct = default);
    }
}
