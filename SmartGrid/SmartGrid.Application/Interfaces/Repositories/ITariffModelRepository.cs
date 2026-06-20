using SmartGrid.Application.Features.Admin;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public record TariffModelSettings(
        double GreenZoneVtPrice,
        double GreenZoneNtPrice,
        double BlueZoneVtPrice,
        double BlueZoneNtPrice,
        double RedZoneVtPrice,
        double RedZoneNtPrice,
        double NetworkCostPerKw,
        double SupplierCost,
        double ApprovedPowerKw,
        double GreenZoneMaxKwh,
        double BlueZoneMaxKwh
    );

    public interface ITariffModelRepository
    {
        Task<TariffModelSettings?> GetActiveAsync(CancellationToken ct = default);
        Task<TariffModelDto> GetAdminModelAsync(CancellationToken ct = default);
        Task<TariffModelDto> SaveAdminModelAsync(TariffModelDto model, CancellationToken ct = default);
    }
}
