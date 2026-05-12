namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

internal sealed class ConsumptionLimitTableEntity : BaseTableEntity
{
    public double LimitKwh { get; set; }
    public double? LimitRsd { get; set; }
}
