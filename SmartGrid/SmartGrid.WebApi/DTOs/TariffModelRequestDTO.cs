namespace SmartGrid.WebApi.DTOs
{
    public class TariffModelRequestDTO
    {
        public required string Name { get; set; }
        public bool IsActive { get; set; }

        public double GreenZoneVtPrice { get; set; }
        public double GreenZoneNtPrice { get; set; }
        public double BlueZoneVtPrice { get; set; }
        public double BlueZoneNtPrice { get; set; }
        public double RedZoneVtPrice { get; set; }
        public double RedZoneNtPrice { get; set; }

        public double NetworkCostPerKw { get; set; }
        public double SupplierCost { get; set; }
        public double ApprovedPowerKw { get; set; }

        public double GreenZoneLimitKwh { get; set; }
        public double BlueZoneLimitKwh { get; set; }
    }
}
