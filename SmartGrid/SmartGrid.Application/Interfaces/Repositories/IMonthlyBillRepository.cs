using SmartGrid.Application.Features.Billing.Commands;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public record BillingPeriodSummary(int Year, int Month, int BillCount, DateTime? LastGeneratedAtUtc);

    public interface IMonthlyBillRepository
    {
        Task SaveOrUpdateAsync(MonthlyBillDto bill, CancellationToken ct = default);
        Task<IReadOnlyCollection<MonthlyBillDto>> GetByPeriodAsync(int year, int month, CancellationToken ct = default);
        Task<MonthlyBillDto?> GetAsync(int year, int month, string deviceId, CancellationToken ct = default);
        Task<IReadOnlyCollection<BillingPeriodSummary>> GetPeriodSummariesAsync(CancellationToken ct = default);
        Task<int> GetTotalBillCountAsync(CancellationToken ct = default);
    }
}
