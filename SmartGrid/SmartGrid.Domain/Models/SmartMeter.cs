using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.Models
{
    public class SmartMeter
    {
        public Guid Id { get; private set; }
        public Guid PropertyId { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public ConnectionType ConnectionType { get; private set; }
        public double MaxApprovedPower { get; private set; }
        public string? Note { get; private set; }
        public string? SerialNumber { get; private set; }
        public PairingStatus PairingStatus { get; private set; }
        public string? DeviceUUID { get; private set; }
        public string? AccessToken { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private SmartMeter() { }

        public static SmartMeter Create(Guid propertyId, string label, ConnectionType connectionType, string? note, DateTime createdAt)
        {
            return new SmartMeter
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                Label = label,
                ConnectionType = connectionType,
                MaxApprovedPower = connectionType == ConnectionType.Monofazni ? 6.9 : 11.04,
                Note = note,
                PairingStatus = PairingStatus.Unpaired,
                CreatedAt = createdAt
            };
        }

        public static SmartMeter Load(
            Guid id, Guid propertyId, string label, ConnectionType connectionType,
            double maxApprovedPower, string? note, string? serialNumber,
            PairingStatus pairingStatus, string? deviceUUID, string? accessToken, DateTime createdAt)
        {
            return new SmartMeter
            {
                Id = id,
                PropertyId = propertyId,
                Label = label,
                ConnectionType = connectionType,
                MaxApprovedPower = maxApprovedPower,
                Note = note,
                SerialNumber = serialNumber,
                PairingStatus = pairingStatus,
                DeviceUUID = deviceUUID,
                AccessToken = accessToken,
                CreatedAt = createdAt
            };
        }

        public void Update(string label, ConnectionType connectionType, string? note)
        {
            Label = label;
            ConnectionType = connectionType;
            MaxApprovedPower = connectionType == ConnectionType.Monofazni ? 6.9 : 11.04;
            Note = note;
        }

        public void RegisterSerialNumber(string serialNumber)
        {
            SerialNumber = serialNumber;
        }

        public void Activate(string deviceUUID, string accessToken)
        {
            DeviceUUID = deviceUUID;
            AccessToken = accessToken;
            PairingStatus = PairingStatus.Paired;
        }
    }
}
