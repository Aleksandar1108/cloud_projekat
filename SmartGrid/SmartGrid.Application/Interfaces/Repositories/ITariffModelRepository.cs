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
        double ApprovedPowerKw
    );

    public interface ITariffModelRepository
    {
        Task<TariffModelSettings?> GetActiveAsync(CancellationToken ct = default);
    }
}
