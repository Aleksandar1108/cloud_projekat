namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class AlertDispatchStateTableEntity : BaseTableEntity
    {
        public DateTime LastNotifiedAtUtc { get; set; }
    }
}
