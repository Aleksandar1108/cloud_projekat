using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public class MonthlyBillRepository(SmartGridDbContext context) : IMonthlyBillRepository
    {
        private readonly SmartGridDbContext _context = context;

        public async Task SaveOrUpdateAsync(MonthlyBillDto bill, CancellationToken ct = default)
        {
            var existing = await _context.MonthlyBills
                .FirstOrDefaultAsync(
                    x => x.DeviceId == bill.DeviceId && x.Year == bill.Year && x.Month == bill.Month,
                    ct);

            if (existing is null)
            {
                await _context.MonthlyBills.AddAsync(new MonthlyBillEntity
                {
                    DeviceId = bill.DeviceId,
                    Year = bill.Year,
                    Month = bill.Month,
                    TotalKwh = bill.TotalKwh,
                    HigherTariffKwh = bill.HigherTariffKwh,
                    LowerTariffKwh = bill.LowerTariffKwh,
                    GreenZoneKwh = bill.GreenZoneKwh,
                    BlueZoneKwh = bill.BlueZoneKwh,
                    RedZoneKwh = bill.RedZoneKwh,
                    EnergyCost = bill.EnergyCost,
                    FixedCosts = bill.FixedCosts,
                    TotalCost = bill.TotalCost,
                    BillText = bill.BillText,
                    GeneratedAtUtc = DateTime.UtcNow
                }, ct);
            }
            else
            {
                existing.TotalKwh = bill.TotalKwh;
                existing.HigherTariffKwh = bill.HigherTariffKwh;
                existing.LowerTariffKwh = bill.LowerTariffKwh;
                existing.GreenZoneKwh = bill.GreenZoneKwh;
                existing.BlueZoneKwh = bill.BlueZoneKwh;
                existing.RedZoneKwh = bill.RedZoneKwh;
                existing.EnergyCost = bill.EnergyCost;
                existing.FixedCosts = bill.FixedCosts;
                existing.TotalCost = bill.TotalCost;
                existing.BillText = bill.BillText;
                existing.GeneratedAtUtc = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyCollection<MonthlyBillDto>> GetByPeriodAsync(int year, int month, CancellationToken ct = default)
        {
            var entities = await _context.MonthlyBills
                .AsNoTracking()
                .Where(x => x.Year == year && x.Month == month)
                .OrderBy(x => x.DeviceId)
                .ToListAsync(ct);

            return entities
                .Select(x => new MonthlyBillDto(
                    x.DeviceId,
                    x.Year,
                    x.Month,
                    x.TotalKwh,
                    x.HigherTariffKwh,
                    x.LowerTariffKwh,
                    x.GreenZoneKwh,
                    x.BlueZoneKwh,
                    x.RedZoneKwh,
                    x.EnergyCost,
                    x.FixedCosts,
                    x.TotalCost,
                    x.BillText))
                .ToList();
        }
    }
}
