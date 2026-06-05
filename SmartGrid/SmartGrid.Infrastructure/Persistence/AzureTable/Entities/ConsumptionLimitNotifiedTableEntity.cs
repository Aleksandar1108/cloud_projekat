namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class ConsumptionLimitNotifiedTableEntity : BaseTableEntity
    {
        public DateTime NotifiedAtUtc { get; set; }
    }
}
