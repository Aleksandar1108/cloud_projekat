using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities
{
    [Table("SmartMeters")]
    public class SmartMeterEntity
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public string Label { get; set; } = null!;
        public string ConnectionType { get; set; } = null!;
        public double MaxApprovedPower { get; set; }
        public string? Note { get; set; }
        public string? SerialNumber { get; set; }
        public string PairingStatus { get; set; } = "Unpaired";
        public string? DeviceUUID { get; set; }
        public string? AccessToken { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
