using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Infrastructure.Common.Options;

namespace SmartGrid.Infrastructure.Services
{
    internal sealed class AzureStorageInitializer(
        TableServiceClient tableServiceClient,
        BlobServiceClient blobServiceClient,
        QueueServiceClient queueServiceClient,
        IOptions<AzureTableOptions> tableOptions,
        IOptions<AzureBlobOptions> blobOptions,
        IOptions<AzureQueueOptions> queueOptions,
        ILogger<AzureStorageInitializer> logger) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var tables = new[]
            {
                tableOptions.Value.TelemetriesTable,
                tableOptions.Value.DevicesTable,
                tableOptions.Value.FirmwaresTable,
                tableOptions.Value.DeviceStatusesTable,
                tableOptions.Value.MonthlyBillsTable,
                tableOptions.Value.ManualReadingsTable,
                tableOptions.Value.PaymentsTable,
                tableOptions.Value.StripeEventsTable,
                tableOptions.Value.ConsumptionLimitsTable,
                tableOptions.Value.ConsumptionLimitNotifiedTable,
                tableOptions.Value.AlertDispatchStateTable
            };

            foreach (var tableName in tables.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
            {
                await tableServiceClient.GetTableClient(tableName).CreateIfNotExistsAsync(cancellationToken);
                logger.LogInformation("Azure table ready: {TableName}", tableName);
            }

            var blobs = new[]
            {
                blobOptions.Value.FirmwareBlob,
                blobOptions.Value.MonthlyBillsBlob,
                blobOptions.Value.ManualReadingsBlob
            };

            foreach (var blobName in blobs.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
            {
                await blobServiceClient.GetBlobContainerClient(blobName).CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                logger.LogInformation("Azure blob container ready: {ContainerName}", blobName);
            }

            var queues = new[]
            {
                queueOptions.Value.DeviceStatusQueue,
                queueOptions.Value.AlertQueue
            };

            foreach (var queueName in queues.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
            {
                await queueServiceClient.GetQueueClient(queueName).CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                logger.LogInformation("Azure queue ready: {QueueName}", queueName);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
