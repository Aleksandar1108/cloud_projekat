namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class ConsumptionLimitTableEntity : BaseTableEntity
    {
        public string Unit { get; set; } = string.Empty;
        public double LimitValue { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
