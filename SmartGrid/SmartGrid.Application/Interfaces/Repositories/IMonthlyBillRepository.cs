using SmartGrid.Application.Features.Billing.Commands;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IMonthlyBillRepository
    {
        Task SaveOrUpdateAsync(MonthlyBillDto bill, CancellationToken ct = default);
        Task<IReadOnlyCollection<MonthlyBillDto>> GetByPeriodAsync(int year, int month, CancellationToken ct = default);
    }
}
