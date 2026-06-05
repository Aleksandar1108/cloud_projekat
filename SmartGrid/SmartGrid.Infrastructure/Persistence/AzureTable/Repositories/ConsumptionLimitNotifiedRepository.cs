using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class ConsumptionLimitNotifiedRepository(
        TableServiceClient tableServiceClient,
        IOptions<AzureTableOptions> options) : IConsumptionLimitNotifiedRepository
    {
        private readonly TableClient _tableClient = tableServiceClient.GetTableClient(options.Value.ConsumptionLimitNotifiedTable);

        public async Task<bool> TryMarkNotifiedAsync(Guid smartMeterId, int year, int month, CancellationToken ct = default)
        {
            var entity = new ConsumptionLimitNotifiedTableEntity
            {
                PartitionKey = smartMeterId.ToString(),
                RowKey = $"{year:D4}-{month:D2}",
                NotifiedAtUtc = DateTime.UtcNow
            };

            try
            {
                await _tableClient.AddEntityAsync(entity, ct);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 409)
            {
                return false;
            }
        }
    }
}
