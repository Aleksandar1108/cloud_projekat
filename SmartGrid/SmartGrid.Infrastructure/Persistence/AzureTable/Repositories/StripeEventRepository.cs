using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class StripeEventRepository(
        TableServiceClient tableServiceClient,
        IOptions<AzureTableOptions> options) : IStripeEventRepository
    {
        private readonly TableClient _tableClient = tableServiceClient.GetTableClient(options.Value.StripeEventsTable);

        public async Task<bool> TryMarkProcessedAsync(string eventId, CancellationToken ct = default)
        {
            var partitionKey = "stripe";
            var rowKey = Uri.EscapeDataString(eventId);

            var entity = new StripeEventTableEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                EventId = eventId,
                ReceivedAtUtc = DateTime.UtcNow
            };

            try
            {
                await _tableClient.AddEntityAsync(entity, ct);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 409)
            {
                // already processed
                return false;
            }
        }
    }
}

