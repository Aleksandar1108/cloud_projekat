using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities
{
    [Table("TariffModels")]
    public class TariffModelEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

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
