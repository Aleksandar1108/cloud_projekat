using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Mappers
{
    public class SmartMeterMapper : IDatabaseMapper<SmartMeter, SmartMeterEntity>
    {
        public SmartMeter? ToDomain(SmartMeterEntity entity)
        {
            return SmartMeter.Load(
                entity.Id,
                entity.PropertyId,
                entity.Label,
                Enum.Parse<ConnectionType>(entity.ConnectionType),
                entity.MaxApprovedPower,
                entity.Note,
                entity.SerialNumber,
                Enum.Parse<PairingStatus>(entity.PairingStatus),
                entity.DeviceUUID,
                entity.AccessToken,
                entity.CreatedAt
            );
        }

        public SmartMeterEntity ToEntity(SmartMeter domain)
        {
            return new SmartMeterEntity
            {
                Id = domain.Id,
                PropertyId = domain.PropertyId,
                Label = domain.Label,
                ConnectionType = domain.ConnectionType.ToString(),
                MaxApprovedPower = domain.MaxApprovedPower,
                Note = domain.Note,
                SerialNumber = domain.SerialNumber,
                PairingStatus = domain.PairingStatus.ToString(),
                DeviceUUID = domain.DeviceUUID,
                AccessToken = domain.AccessToken,
                CreatedAt = domain.CreatedAt
            };
        }
    }
}
