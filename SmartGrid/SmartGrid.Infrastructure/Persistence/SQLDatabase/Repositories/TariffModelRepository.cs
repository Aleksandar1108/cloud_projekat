using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Interfaces.Repositories;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public class TariffModelRepository(SmartGridDbContext context) : ITariffModelRepository
    {
        private readonly SmartGridDbContext _context = context;

        public async Task<TariffModelSettings?> GetActiveAsync(CancellationToken ct = default)
        {
            var activeModel = await _context.TariffModels
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.IsActive, ct);

            if (activeModel is null)
            {
                return null;
            }

            return new TariffModelSettings(
                activeModel.GreenZoneVtPrice,
                activeModel.GreenZoneNtPrice,
                activeModel.BlueZoneVtPrice,
                activeModel.BlueZoneNtPrice,
                activeModel.RedZoneVtPrice,
                activeModel.RedZoneNtPrice,
                activeModel.NetworkCostPerKw,
                activeModel.SupplierCost,
                activeModel.ApprovedPowerKw);
        }
    }
}
