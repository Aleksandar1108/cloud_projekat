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

        private readonly Dictionary<string, DateTime> _lastTimestampByDevice = new();

        public TelemetryDTO GenerateTelemetry(
            string deviceId,
            string deviceName,
            double nominalPower,
            string firmwareVersion,
            DeviceType deviceType)
        {

            double currentPower = _random.NextDouble() * (nominalPower + _maxPowerVariation);

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

            DateTime timestamp;

            if (!_lastTimestampByDevice.ContainsKey(deviceId))
            {
                timestamp = DateTime.UtcNow.AddDays(-5);
            }
            else
            {
                timestamp = _lastTimestampByDevice[deviceId].AddHours(2);
            }

            _lastTimestampByDevice[deviceId] = timestamp;

            return new TelemetryDTO
            {
                DeviceId = deviceId,
                DeviceName = deviceName,
                DeviceType = deviceType,
                NominalPower = nominalPower,
                CurrentPower = currentPower,
                FirmwareVersion = firmwareVersion,
                Timestamp = timestamp,
                Voltage = voltage,
                TotalConsumption = Math.Round(_totalConsumptionByDevice[deviceId], 3)
            };
        }
    }
}