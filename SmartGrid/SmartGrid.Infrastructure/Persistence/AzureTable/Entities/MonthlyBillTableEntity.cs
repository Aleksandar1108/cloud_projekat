namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class MonthlyBillTableEntity : BaseTableEntity
    {
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
        public string BillText { get; set; } = string.Empty;
        public DateTime GeneratedAtUtc { get; set; }
    }
}
