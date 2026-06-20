using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Features.Payments;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class MonthlyBillRepository(
        TableServiceClient tableServiceClient,
        ITableKeyProvider<MonthlyBillDto> keyProvider,
        ITableMapper<MonthlyBillDto, MonthlyBillTableEntity> mapper,
        IOptions<AzureTableOptions> options)
        : AzureTableRepository<MonthlyBillDto, MonthlyBillTableEntity>(
            tableServiceClient.GetTableClient(options.Value.MonthlyBillsTable),
            keyProvider,
            mapper),
          IMonthlyBillRepository
    {
        public async Task SaveOrUpdateAsync(MonthlyBillDto bill, CancellationToken ct = default)
        {
            await base.UpsertAsync(bill, ct);
        }

        public async Task<IReadOnlyCollection<MonthlyBillDto>> GetByPeriodAsync(int year, int month, CancellationToken ct = default)
        {
            var partitionKey = $"{year:D4}-{month:D2}";
            List<MonthlyBillDto> bills;

            try
            {
                bills = await base.QueryByPartitionKeyAsync(partitionKey, ct);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return Array.Empty<MonthlyBillDto>();
            }

            return bills.OrderBy(x => x.DeviceId).ToList();
        }

        public async Task<MonthlyBillDto?> GetAsync(int year, int month, string deviceId, CancellationToken ct = default)
        {
            var partitionKey = $"{year:D4}-{month:D2}";
            var rowKey = Uri.EscapeDataString(deviceId);
            return await base.GetByIdAsync(partitionKey, rowKey, ct);
        }

        public async Task<IReadOnlyCollection<BillingPeriodSummary>> GetPeriodSummariesAsync(CancellationToken ct = default)
        {
            try
            {
                var query = _tableClient.QueryAsync<MonthlyBillTableEntity>(cancellationToken: ct);
                var grouped = new Dictionary<string, (int Count, DateTime? LastGenerated)>();

                await foreach (var entity in query)
                {
                    var key = entity.PartitionKey;
                    if (!grouped.TryGetValue(key, out var current))
                    {
                        grouped[key] = (1, entity.GeneratedAtUtc);
                        continue;
                    }

                    var lastGenerated = current.LastGenerated;
                    if (entity.GeneratedAtUtc > (lastGenerated ?? DateTime.MinValue))
                    {
                        lastGenerated = entity.GeneratedAtUtc;
                    }

                    grouped[key] = (current.Count + 1, lastGenerated);
                }

                return grouped
                    .Select(x =>
                    {
                        var parts = x.Key.Split('-');
                        var year = int.Parse(parts[0]);
                        var month = int.Parse(parts[1]);
                        return new BillingPeriodSummary(year, month, x.Value.Count, x.Value.LastGenerated);
                    })
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .ToList();
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return Array.Empty<BillingPeriodSummary>();
            }
        }

        public async Task<int> GetTotalBillCountAsync(CancellationToken ct = default)
        {
            try
            {
                var query = _tableClient.QueryAsync<MonthlyBillTableEntity>(
                    select: new[] { "PartitionKey" },
                    cancellationToken: ct);

                var count = 0;
                await foreach (var _ in query)
                {
                    count++;
                }

                return count;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return 0;
            }
        }
    }
}
