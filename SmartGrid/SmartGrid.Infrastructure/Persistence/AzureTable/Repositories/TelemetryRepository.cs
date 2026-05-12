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
                return Array.Empty<Telemetry>();
            }
        }

        public async Task<double> GetEnergyDeltaKwhSumForDeviceUtcMonthAsync(
            string deviceId,
            int year,
            int month,
            CancellationToken ct = default)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);
            var startS = start.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);
            var endS = end.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);
            var escapedDevice = deviceId.Replace("'", "''");
            var filter =
                $"PartitionKey eq '{escapedDevice}' and ObservationTime ge datetime'{startS}' and ObservationTime lt datetime'{endS}'";

            try
            {
                var rows = await base.QueryAsync(filter, ct);
                return rows.Sum(t => t.EnergyDeltaKwh ?? 0);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return 0;
            }
        }
    }
}
