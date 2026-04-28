using SmartGrid.Application.Features.ManualReadings;
using SmartGrid.Domain.Enums;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Mappers
{
    internal class ManualReadingTableMapper : ITableMapper<ManualReadingDto, ManualReadingTableEntity>
    {
        public ManualReadingTableEntity ToEntity(ManualReadingDto domain)
        {
            return new ManualReadingTableEntity
            {
                DeviceId = domain.DeviceId,
                ReadingKwh = domain.ReadingKwh,
                ReadingAtUtc = domain.ReadingAtUtc,
                SubmitterEmail = domain.SubmitterEmail,
                Status = domain.Status.ToString(),
                RawImagePath = domain.RawImagePath,
                OptimizedImagePath = domain.OptimizedImagePath,
                CreatedAtUtc = domain.CreatedAtUtc
            };
        }

        public ManualReadingDto? ToDomain(ManualReadingTableEntity entity)
        {
            Enum.TryParse<ManualReadingStatus>(entity.Status, true, out var parsedStatus);
            return new ManualReadingDto(
                Guid.Parse(entity.RowKey),
                entity.DeviceId,
                entity.ReadingKwh,
                entity.ReadingAtUtc,
                entity.SubmitterEmail,
                parsedStatus,
                entity.RawImagePath,
                entity.OptimizedImagePath,
                entity.CreatedAtUtc
            );
        }
    }
}
