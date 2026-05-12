namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

internal sealed class TariffModelTableEntity : BaseTableEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public double GreenZoneVtPrice { get; set; }
    public double GreenZoneNtPrice { get; set; }
    public double BlueZoneVtPrice { get; set; }
    public double BlueZoneNtPrice { get; set; }
    public double RedZoneVtPrice { get; set; }
    public double RedZoneNtPrice { get; set; }
    public double NetworkCostPerKw { get; set; }
    public double SupplierCost { get; set; }
    public double ApprovedPowerKw { get; set; }
}
