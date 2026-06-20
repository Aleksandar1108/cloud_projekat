using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Features.Admin;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public class TariffModelRepository(SmartGridDbContext context) : ITariffModelRepository
    {
        private const double DefaultGreenMaxKwh = 350;
        private const double DefaultBlueMaxKwh = 1200;

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

            return MapToSettings(activeModel);
        }

        public async Task<TariffModelDto> GetAdminModelAsync(CancellationToken ct = default)
        {
            var activeModel = await _context.TariffModels
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.IsActive, ct);

            if (activeModel is null)
            {
                return CreateDefaultDto();
            }

            return MapToDto(activeModel);
        }

        public async Task<TariffModelDto> SaveAdminModelAsync(TariffModelDto model, CancellationToken ct = default)
        {
            var activeModel = await _context.TariffModels
                .FirstOrDefaultAsync(x => x.IsActive, ct);

            if (activeModel is null)
            {
                activeModel = new TariffModelEntity
                {
                    Name = string.IsNullOrWhiteSpace(model.Name) ? "Standardni tarifni model" : model.Name,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ApprovedPowerKw = 6.9,
                    GreenZoneMaxKwh = model.ZoneThresholds.GreenMaxKwh,
                    BlueZoneMaxKwh = model.ZoneThresholds.BlueMaxKwh,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.TariffModels.Add(activeModel);
            }

            activeModel.Name = string.IsNullOrWhiteSpace(model.Name) ? activeModel.Name : model.Name;
            activeModel.GreenZoneMaxKwh = model.ZoneThresholds.GreenMaxKwh;
            activeModel.BlueZoneMaxKwh = model.ZoneThresholds.BlueMaxKwh;
            activeModel.GreenZoneVtPrice = model.ZonePrices.Green.VtPriceRsd;
            activeModel.GreenZoneNtPrice = model.ZonePrices.Green.NtPriceRsd;
            activeModel.BlueZoneVtPrice = model.ZonePrices.Blue.VtPriceRsd;
            activeModel.BlueZoneNtPrice = model.ZonePrices.Blue.NtPriceRsd;
            activeModel.RedZoneVtPrice = model.ZonePrices.Red.VtPriceRsd;
            activeModel.RedZoneNtPrice = model.ZonePrices.Red.NtPriceRsd;
            activeModel.NetworkCostPerKw = model.FixedCosts.BillingPowerFeeRsd;
            activeModel.SupplierCost = model.FixedCosts.SupplierCostRsd;
            activeModel.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return MapToDto(activeModel);
        }

        private static TariffModelDto CreateDefaultDto()
        {
            var now = DateTime.UtcNow;
            return new TariffModelDto(
                "default",
                "Standardni tarifni model",
                now.ToString("yyyy-MM-dd"),
                new TariffZoneThresholdsDto(DefaultGreenMaxKwh, DefaultBlueMaxKwh),
                new TariffZonePricesDto(
                    new TariffZonePriceDto(8.5, 4.2),
                    new TariffZonePriceDto(11.0, 6.5),
                    new TariffZonePriceDto(14.5, 9.0)),
                new TariffFixedCostsDto(350, 280),
                now.ToString("o"));
        }

        private static TariffModelSettings MapToSettings(TariffModelEntity entity)
        {
            return new TariffModelSettings(
                entity.GreenZoneVtPrice,
                entity.GreenZoneNtPrice,
                entity.BlueZoneVtPrice,
                entity.BlueZoneNtPrice,
                entity.RedZoneVtPrice,
                entity.RedZoneNtPrice,
                entity.NetworkCostPerKw,
                entity.SupplierCost,
                entity.ApprovedPowerKw,
                ResolveGreenMax(entity),
                ResolveBlueMax(entity));
        }

        private static TariffModelDto MapToDto(TariffModelEntity entity)
        {
            return new TariffModelDto(
                entity.Id.ToString(),
                entity.Name,
                entity.CreatedAt.ToString("yyyy-MM-dd"),
                new TariffZoneThresholdsDto(ResolveGreenMax(entity), ResolveBlueMax(entity)),
                new TariffZonePricesDto(
                    new TariffZonePriceDto(entity.GreenZoneVtPrice, entity.GreenZoneNtPrice),
                    new TariffZonePriceDto(entity.BlueZoneVtPrice, entity.BlueZoneNtPrice),
                    new TariffZonePriceDto(entity.RedZoneVtPrice, entity.RedZoneNtPrice)),
                new TariffFixedCostsDto(entity.NetworkCostPerKw, entity.SupplierCost),
                (entity.UpdatedAt == default ? entity.CreatedAt : entity.UpdatedAt).ToString("o"));
        }

        private static double ResolveGreenMax(TariffModelEntity entity) =>
            entity.GreenZoneMaxKwh > 0 ? entity.GreenZoneMaxKwh : DefaultGreenMaxKwh;

        private static double ResolveBlueMax(TariffModelEntity entity) =>
            entity.BlueZoneMaxKwh > ResolveGreenMax(entity)
                ? entity.BlueZoneMaxKwh
                : DefaultBlueMaxKwh;
    }
}
