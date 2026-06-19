using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

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
                activeModel.ApprovedPowerKw,
                activeModel.GreenZoneLimitKwh,
                activeModel.BlueZoneLimitKwh);
        }

        public async Task<IReadOnlyList<TariffModelDto>> GetAllAsync(CancellationToken ct = default)
        {
            var models = await _context.TariffModels
                .AsNoTracking()
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

            return models.Select(ToDto).ToList();
        }

        public async Task<TariffModelDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var model = await _context.TariffModels
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            return model is null ? null : ToDto(model);
        }

        public async Task<int> CreateAsync(TariffModelDto model, CancellationToken ct = default)
        {
            var entity = new TariffModelEntity
            {
                Name = model.Name,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                GreenZoneVtPrice = model.GreenZoneVtPrice,
                GreenZoneNtPrice = model.GreenZoneNtPrice,
                BlueZoneVtPrice = model.BlueZoneVtPrice,
                BlueZoneNtPrice = model.BlueZoneNtPrice,
                RedZoneVtPrice = model.RedZoneVtPrice,
                RedZoneNtPrice = model.RedZoneNtPrice,
                NetworkCostPerKw = model.NetworkCostPerKw,
                SupplierCost = model.SupplierCost,
                ApprovedPowerKw = model.ApprovedPowerKw,
                GreenZoneLimitKwh = model.GreenZoneLimitKwh,
                BlueZoneLimitKwh = model.BlueZoneLimitKwh
            };

            if (entity.IsActive)
            {
                await DeactivateAllAsync(ct);
            }

            _context.TariffModels.Add(entity);
            await _context.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(TariffModelDto model, CancellationToken ct = default)
        {
            var entity = await _context.TariffModels.FirstOrDefaultAsync(x => x.Id == model.Id, ct);
            if (entity is null)
            {
                return false;
            }

            entity.Name = model.Name;
            entity.GreenZoneVtPrice = model.GreenZoneVtPrice;
            entity.GreenZoneNtPrice = model.GreenZoneNtPrice;
            entity.BlueZoneVtPrice = model.BlueZoneVtPrice;
            entity.BlueZoneNtPrice = model.BlueZoneNtPrice;
            entity.RedZoneVtPrice = model.RedZoneVtPrice;
            entity.RedZoneNtPrice = model.RedZoneNtPrice;
            entity.NetworkCostPerKw = model.NetworkCostPerKw;
            entity.SupplierCost = model.SupplierCost;
            entity.ApprovedPowerKw = model.ApprovedPowerKw;
            entity.GreenZoneLimitKwh = model.GreenZoneLimitKwh;
            entity.BlueZoneLimitKwh = model.BlueZoneLimitKwh;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> SetActiveAsync(int id, CancellationToken ct = default)
        {
            var target = await _context.TariffModels.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (target is null)
            {
                return false;
            }

            await DeactivateAllAsync(ct);

            target.IsActive = true;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task DeactivateAllAsync(CancellationToken ct)
        {
            var activeModels = await _context.TariffModels
                .Where(x => x.IsActive)
                .ToListAsync(ct);

            foreach (var model in activeModels)
            {
                model.IsActive = false;
            }
        }

        private static TariffModelDto ToDto(TariffModelEntity entity)
            => new(
                entity.Id,
                entity.Name,
                entity.IsActive,
                entity.CreatedAt,
                entity.GreenZoneVtPrice,
                entity.GreenZoneNtPrice,
                entity.BlueZoneVtPrice,
                entity.BlueZoneNtPrice,
                entity.RedZoneVtPrice,
                entity.RedZoneNtPrice,
                entity.NetworkCostPerKw,
                entity.SupplierCost,
                entity.ApprovedPowerKw,
                entity.GreenZoneLimitKwh,
                entity.BlueZoneLimitKwh);
    }
}
