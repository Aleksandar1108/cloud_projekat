using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class AlertDispatchStateRepository(
        TableServiceClient tableServiceClient,
        IOptions<AzureTableOptions> options) : IAlertDispatchStateRepository
    {
        private readonly TableClient _tableClient = tableServiceClient.GetTableClient(options.Value.AlertDispatchStateTable);

        public async Task<bool> ShouldNotifyAsync(
            string deviceId,
            string alertKind,
            TimeSpan cooldown,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            try
            {
                var response = await _tableClient.GetEntityAsync<AlertDispatchStateTableEntity>(
                    deviceId,
                    alertKind,
                    cancellationToken: ct);

                if (now - response.Value.LastNotifiedAtUtc < cooldown)
                {
                    return false;
                }
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
            }

            var entity = new AlertDispatchStateTableEntity
            {
                PartitionKey = deviceId,
                RowKey = alertKind,
                LastNotifiedAtUtc = now
            };

            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
            return true;
        }

        public async Task ClearAsync(string deviceId, string alertKind, CancellationToken ct = default)
        {
            try
            {
                await _tableClient.DeleteEntityAsync(deviceId, alertKind, cancellationToken: ct);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
            }
        }
    }
}
