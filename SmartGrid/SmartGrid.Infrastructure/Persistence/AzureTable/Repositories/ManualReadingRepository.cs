using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Features.ManualReadings;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Enums;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class ManualReadingRepository(
        TableServiceClient tableServiceClient,
        ITableKeyProvider<ManualReadingDto> keyProvider,
        ITableMapper<ManualReadingDto, ManualReadingTableEntity> mapper,
        IOptions<AzureTableOptions> options)
        : AzureTableRepository<ManualReadingDto, ManualReadingTableEntity>(
            tableServiceClient.GetTableClient(options.Value.ManualReadingsTable),
            keyProvider,
            mapper),
          IManualReadingRepository
    {
        private readonly TableClient _manualReadingsTableClient = tableServiceClient.GetTableClient(options.Value.ManualReadingsTable);

        public async Task<ManualReadingDto> CreateAsync(ManualReadingDto reading, CancellationToken ct = default)
        {
            await base.AddAsync(reading, ct);
            return reading;
        }

        public async Task<ManualReadingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var pending = await base.GetByIdAsync(ManualReadingStatus.Pending.ToString(), id.ToString(), ct);
            if (pending is not null) return pending;
            return await base.GetByIdAsync(ManualReadingStatus.Processed.ToString(), id.ToString(), ct);
        }

        public async Task<IReadOnlyCollection<ManualReadingDto>> GetAllAsync(ManualReadingStatus? status = null, CancellationToken ct = default)
        {
            if (status.HasValue)
            {
                return await base.QueryByPartitionKeyAsync(status.Value.ToString(), ct);
            }

            var pending = await base.QueryByPartitionKeyAsync(ManualReadingStatus.Pending.ToString(), ct);
            var processed = await base.QueryByPartitionKeyAsync(ManualReadingStatus.Processed.ToString(), ct);
            return pending.Concat(processed).OrderByDescending(x => x.CreatedAtUtc).ToList();
        }

        public async Task<bool> MarkProcessedAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await base.GetByIdAsync(ManualReadingStatus.Pending.ToString(), id.ToString(), ct);
            if (existing is null) return false;

            await _manualReadingsTableClient.DeleteEntityAsync(ManualReadingStatus.Pending.ToString(), id.ToString(), cancellationToken: ct);
            await base.AddAsync(existing with { Status = ManualReadingStatus.Processed }, ct);
            return true;
        }

        public async Task<bool> DeletePendingAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await base.GetByIdAsync(ManualReadingStatus.Pending.ToString(), id.ToString(), ct);
            if (existing is null) return false;

            await _manualReadingsTableClient.DeleteEntityAsync(ManualReadingStatus.Pending.ToString(), id.ToString(), cancellationToken: ct);
            return true;
        }

        public async Task<IReadOnlyCollection<ManualReadingDto>> GetProcessedByPeriodAsync(DateTime periodStartUtc, DateTime periodEndUtc, CancellationToken ct = default)
        {
            var processed = await base.QueryByPartitionKeyAsync(ManualReadingStatus.Processed.ToString(), ct);
            return processed
                .Where(x => x.ReadingAtUtc >= periodStartUtc && x.ReadingAtUtc < periodEndUtc)
                .ToList();
        }
    }
}
