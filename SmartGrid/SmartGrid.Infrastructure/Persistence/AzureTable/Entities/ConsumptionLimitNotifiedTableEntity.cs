namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

internal sealed class ConsumptionLimitNotifiedTableEntity : BaseTableEntity
{
    public DateTime NotifiedAtUtc { get; set; }
}
