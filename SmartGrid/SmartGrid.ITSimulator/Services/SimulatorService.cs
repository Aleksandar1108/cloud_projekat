using SmartGrid.ITSimulator.Enums;
using SmartGrid.ITSimulator.Models;

namespace SmartGrid.ITSimulator.Services
{
    public class SimulatorService
    {
        private readonly Random _random;
        private readonly double _maxPowerVariation;

        private readonly Dictionary<string, double> _totalConsumptionByDevice = new();

        public SimulatorService(double maxPowerVariation = 50)
        {
            _random = new Random();
            _maxPowerVariation = maxPowerVariation;
        }

        public TelemetryDTO GenerateTelemetry(
            string deviceId,
            string deviceName,
            double nominalPower,
            string firmwareVersion,
            DeviceType deviceType)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException( "Device Id cannot be empty", nameof(deviceId));

            if (string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentException("Device Name cannot be empty",nameof(deviceName));

            if (string.IsNullOrWhiteSpace(firmwareVersion))
                throw new ArgumentException("Firmware Version cannot be empty", nameof(firmwareVersion));

            if (nominalPower <= 0)
                throw new ArgumentException( "Nominal power must be greater than zero", nameof(nominalPower));

            double currentPower = _random.NextDouble() *(nominalPower + _maxPowerVariation);

            currentPower = Math.Min(currentPower, nominalPower * 1.3);

            double voltage = deviceType == DeviceType.Monofazni
                ? 220 + (_random.NextDouble() * 20 - 10)
                : 400 + (_random.NextDouble() * 20 - 10);

            if (_random.Next(100) < 3)
            {
                voltage = 180;
            }

            if (!_totalConsumptionByDevice.ContainsKey(deviceId))
            {
                _totalConsumptionByDevice[deviceId] =
                    _random.Next(1000, 5000);
            }


            double generatedConsumption = currentPower * (10.0 / 3600.0);

            _totalConsumptionByDevice[deviceId] += generatedConsumption;

            return new TelemetryDTO
            {
                DeviceId = deviceId,
                DeviceName = deviceName,
                DeviceType = deviceType,
                NominalPower = nominalPower,
                CurrentPower = currentPower,
                FirmwareVersion = firmwareVersion,
                Timestamp = DateTime.UtcNow,
                Voltage = voltage,
                TotalConsumption =  Math.Round( _totalConsumptionByDevice[deviceId], 3)
            };
        }
    }
}