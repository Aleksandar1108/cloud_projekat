using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Models
{
    public class Telemetry
    {
        public EntityId Id { get; private set; }
        public EntityId DeviceId { get; private set; }
        public string DeviceName { get; private set; } = string.Empty;
        public DeviceType DeviceType { get; private set; } = DeviceType.Unknown;
        public FirmwareVersion FirmwareVersion { get; private set; }
        public Power NominalPower { get; private set; }
        public Power CurrentPower { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Percentage LoadPercentage { get; private set; }

        /// <summary>Optional phase voltages (V). Mono typically sends U1 only.</summary>
        public double? VoltageVoltsU1 { get; private set; }
        public double? VoltageVoltsU2 { get; private set; }
        public double? VoltageVoltsU3 { get; private set; }

        /// <summary>Energy consumed in this interval (kWh), for monthly limit checks.</summary>
        public double? EnergyDeltaKwh { get; private set; }

        private Telemetry(
            EntityId id,
            EntityId deviceId,
            string deviceName,
            DeviceType deviceType,
            Power nominalPower,
            Power currentPower,
            DateTime timestamp,
            FirmwareVersion firmwareVersion,
            double? voltageVoltsU1,
            double? voltageVoltsU2,
            double? voltageVoltsU3,
            double? energyDeltaKwh)
        {
            Id = id;
            DeviceId = deviceId;
            DeviceName = deviceName;
            DeviceType = deviceType;
            NominalPower = nominalPower;
            CurrentPower = currentPower;
            Timestamp = timestamp;
            FirmwareVersion = firmwareVersion;
            VoltageVoltsU1 = voltageVoltsU1;
            VoltageVoltsU2 = voltageVoltsU2;
            VoltageVoltsU3 = voltageVoltsU3;
            EnergyDeltaKwh = energyDeltaKwh;
            LoadPercentage = nominalPower.Value > 0
                ? Percentage.FromRaw(Math.Round((currentPower.Value / nominalPower.Value) * 100, 2))
                : Percentage.Zero();
        }

        public double? GetMinimumReportedVoltageVolts()
        {
            var values = new List<double>(3);
            if (VoltageVoltsU1.HasValue) values.Add(VoltageVoltsU1.Value);
            if (VoltageVoltsU2.HasValue) values.Add(VoltageVoltsU2.Value);
            if (VoltageVoltsU3.HasValue) values.Add(VoltageVoltsU3.Value);
            return values.Count == 0 ? null : values.Min();
        }

        #region Factory Method

        public static Result<Telemetry> Create(
            string deviceId,
            string deviceName,
            DeviceType deviceType,
            double nominalPower,
            double currentPower,
            DateTime timestamp,
            string firmwareVersion,
            double? voltageVoltsU1 = null,
            double? voltageVoltsU2 = null,
            double? voltageVoltsU3 = null,
            double? energyDeltaKwh = null)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                return Result<Telemetry>.Failure("DeviceName is required.",
                    ErrorType.Validation);

            if (!Enum.IsDefined(typeof(DeviceType), deviceType)
                || deviceType == DeviceType.Unknown)
                return Result<Telemetry>.Failure("A valid and defined DeviceType must be specified.",
                    ErrorType.Validation);

            if (timestamp > DateTime.UtcNow)
                return Result<Telemetry>.Failure("Timestamp cannot be in the future.",
                    ErrorType.Validation);

            var idResult = EntityId.Create(deviceId);

            if (idResult.IsFailure)
                return Result<Telemetry>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var nominalPowerResult = Power.Create(nominalPower);

            if (nominalPowerResult.IsFailure)
                return Result<Telemetry>.Failure(nominalPowerResult.Error!.Message, ErrorType.Validation);

            var currentPowerResult = Power.Create(currentPower);

            if (currentPowerResult.IsFailure)
                return Result<Telemetry>.Failure(currentPowerResult.Error!.Message, ErrorType.Validation);

            var firmwareVersionResult = FirmwareVersion.Create(firmwareVersion);

            if (firmwareVersionResult.IsFailure)
                return Result<Telemetry>.Failure(firmwareVersionResult.Error!.Message, ErrorType.Validation);

            if (energyDeltaKwh is < 0)
                return Result<Telemetry>.Failure("EnergyDeltaKwh cannot be negative.", ErrorType.Validation);

            if (voltageVoltsU1 is < 0 or > 500 || voltageVoltsU2 is < 0 or > 500 || voltageVoltsU3 is < 0 or > 500)
                return Result<Telemetry>.Failure("Voltage values must be between 0 and 500 V.", ErrorType.Validation);

            return Result<Telemetry>.Success(new Telemetry(
                EntityId.New(),
                idResult.Value,
                deviceName,
                deviceType,
                nominalPowerResult.Value,
                currentPowerResult.Value,
                timestamp,
                firmwareVersionResult.Value,
                voltageVoltsU1,
                voltageVoltsU2,
                voltageVoltsU3,
                energyDeltaKwh
            ));
        }
        public static Result<Telemetry> Load(
            string id,
            string deviceId,
            string deviceName,
            DeviceType deviceType,
            double nominalPower,
            double currentPower,
            DateTime timestamp,
            string firmwareVersion,
            double? voltageVoltsU1 = null,
            double? voltageVoltsU2 = null,
            double? voltageVoltsU3 = null,
            double? energyDeltaKwh = null)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Telemetry>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var deviceIdResult = EntityId.Create(deviceId);
            if (deviceIdResult.IsFailure)
                return Result<Telemetry>.Failure(deviceIdResult.Error!.Message, ErrorType.Validation);

            var nominalPowerResult = Power.Create(nominalPower);
            if (nominalPowerResult.IsFailure)
                return Result<Telemetry>.Failure(nominalPowerResult.Error!.Message, ErrorType.Validation);

            var currentPowerResult = Power.Create(currentPower);
            if (currentPowerResult.IsFailure)
                return Result<Telemetry>.Failure(currentPowerResult.Error!.Message, ErrorType.Validation);

            var firmwareResult = FirmwareVersion.Create(firmwareVersion);
            if (firmwareResult.IsFailure)
                return Result<Telemetry>.Failure(firmwareResult.Error!.Message, ErrorType.Validation);

            if (energyDeltaKwh is < 0)
                return Result<Telemetry>.Failure("EnergyDeltaKwh cannot be negative.", ErrorType.Validation);

            if (voltageVoltsU1 is < 0 or > 500 || voltageVoltsU2 is < 0 or > 500 || voltageVoltsU3 is < 0 or > 500)
                return Result<Telemetry>.Failure("Voltage values must be between 0 and 500 V.", ErrorType.Validation);

            var telemetry = new Telemetry(
                idResult.Value,
                deviceIdResult.Value,
                deviceName,
                deviceType,
                nominalPowerResult.Value,
                currentPowerResult.Value,
                timestamp,
                firmwareResult.Value,
                voltageVoltsU1,
                voltageVoltsU2,
                voltageVoltsU3,
                energyDeltaKwh);

            return Result<Telemetry>.Success(telemetry);
        }

        #endregion
    }
}