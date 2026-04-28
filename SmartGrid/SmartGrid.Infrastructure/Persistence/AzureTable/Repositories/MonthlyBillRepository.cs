using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Features.Billing.Commands;
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
    }
}
