using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities
{
    [Table("MonthlyBills")]
    public class MonthlyBillEntity
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = null!;
        public int Year { get; set; }
        public int Month { get; set; }
        public double TotalKwh { get; set; }
        public double HigherTariffKwh { get; set; }
        public double LowerTariffKwh { get; set; }
        public double GreenZoneKwh { get; set; }
        public double BlueZoneKwh { get; set; }
        public double RedZoneKwh { get; set; }
        public double EnergyCost { get; set; }
        public double FixedCosts { get; set; }
        public double TotalCost { get; set; }
        public string BillText { get; set; } = null!;
        public DateTime GeneratedAtUtc { get; set; }
    }
}
