namespace SmartGrid.ITSimulator.Models
{
    public class SmartMeterDto
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string ConnectionType { get; set; } = string.Empty;
        public double MaxApprovedPower { get; set; }
        public string? Note { get; set; }
        public string? SerialNumber { get; set; }
        public string PairingStatus { get; set; } = string.Empty;
        public string? DeviceUUID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}