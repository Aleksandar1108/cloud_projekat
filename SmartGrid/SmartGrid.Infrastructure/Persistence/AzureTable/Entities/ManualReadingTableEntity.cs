namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class ManualReadingTableEntity : BaseTableEntity
    {
        public string DeviceId { get; set; } = string.Empty;
        public string MeterName { get; set; } = string.Empty;
        public double ReadingKwh { get; set; }
        public DateTime ReadingAtUtc { get; set; }
        public string SubmitterEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RawImagePath { get; set; } = string.Empty;
        public string OptimizedImagePath { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
