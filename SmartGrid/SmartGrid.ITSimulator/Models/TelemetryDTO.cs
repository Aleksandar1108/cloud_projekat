using SmartGrid.ITSimulator.Enums;

namespace SmartGrid.ITSimulator.Models
{
    public class TelemetryDTO
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public DeviceType DeviceType { get; set; }
        public double NominalPower { get; set; }
        public double CurrentPower { get; set; }
        public string FirmwareVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        public double? Voltage { get; set; }

        public double TotalConsumption { get; set; }
    }
}