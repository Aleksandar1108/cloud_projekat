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
        double GreenZoneLimitKwh,
        double BlueZoneLimitKwh
    );

    public record TariffModelDto(
        int Id,
        string Name,
        bool IsActive,
        DateTime CreatedAt,
        double GreenZoneVtPrice,
        double GreenZoneNtPrice,
        double BlueZoneVtPrice,
        double BlueZoneNtPrice,
        double RedZoneVtPrice,
        double RedZoneNtPrice,
        double NetworkCostPerKw,
        double SupplierCost,
        double ApprovedPowerKw,
        double GreenZoneLimitKwh,
        double BlueZoneLimitKwh
    );

    public interface ITariffModelRepository
    {
        Task<TariffModelSettings?> GetActiveAsync(CancellationToken ct = default);

        Task<IReadOnlyList<TariffModelDto>> GetAllAsync(CancellationToken ct = default);

        Task<TariffModelDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<int> CreateAsync(TariffModelDto model, CancellationToken ct = default);

        Task<bool> UpdateAsync(TariffModelDto model, CancellationToken ct = default);

        Task<bool> SetActiveAsync(int id, CancellationToken ct = default);
    }
}
