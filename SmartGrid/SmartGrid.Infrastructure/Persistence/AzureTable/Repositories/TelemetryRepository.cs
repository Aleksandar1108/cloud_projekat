using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;
using System.Globalization;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class TelemetryRepository(
        TableServiceClient tableServiceClient,
        ITableKeyProvider<Telemetry> keyProvider,
        ITableMapper<Telemetry, TelemetryEntity> mapper,
        IOptions<AzureTableOptions> options
    ) : AzureTableRepository<Telemetry, TelemetryEntity>(
              tableServiceClient.GetTableClient(options.Value.TelemetriesTable),
              keyProvider,
              mapper),
        ITelemetryRepository
    {
        public async Task SaveAsync(Telemetry telemetry, CancellationToken ct)
        {
            await base.AddAsync(telemetry, ct);
        }

        public async Task<IReadOnlyCollection<Telemetry>> GetByPeriodAsync(DateTime periodStartUtc, DateTime periodEndUtc, CancellationToken ct = default)
        {
            var start = periodStartUtc.ToString("O", CultureInfo.InvariantCulture);
            var end = periodEndUtc.ToString("O", CultureInfo.InvariantCulture);
            string filter = $"ObservationTime ge datetime'{start}' and ObservationTime lt datetime'{end}'";

            try
            {
                var telemetry = await base.QueryAsync(filter, ct);

                return telemetry;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // If Telemetries table is not created yet, treat it as no data for billing.
                return Array.Empty<Telemetry>();
            }
        }

        public async Task<IReadOnlyCollection<Telemetry>> GetByDeviceAndPeriodAsync(
            string deviceId,
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            CancellationToken ct = default)
        {
            var start = periodStartUtc.ToString("O", CultureInfo.InvariantCulture);
            var end = periodEndUtc.ToString("O", CultureInfo.InvariantCulture);
            var escapedDeviceId = deviceId.Replace("'", "''");
            string filter =
                $"PartitionKey eq '{escapedDeviceId}' and ObservationTime ge datetime'{start}' and ObservationTime lt datetime'{end}'";

            try
            {
                return await base.QueryAsync(filter, ct);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return Array.Empty<Telemetry>();
            }
        }

        public async Task<Telemetry?> GetLatestByDeviceIdAsync(string deviceId, CancellationToken ct = default)
        {
            var escapedDeviceId = deviceId.Replace("'", "''");
            string filter = $"PartitionKey eq '{escapedDeviceId}'";

            try
            {
                var samples = await base.QueryAsync(filter, ct);
                return samples
                    .OrderByDescending(x => x.Timestamp)
                    .FirstOrDefault();
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }
    }
}
