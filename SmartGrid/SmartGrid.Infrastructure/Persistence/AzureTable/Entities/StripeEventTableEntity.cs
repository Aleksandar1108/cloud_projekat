namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class StripeEventTableEntity : BaseTableEntity
    {
        public string EventId { get; set; } = string.Empty;
        public DateTime ReceivedAtUtc { get; set; }
    }
}

