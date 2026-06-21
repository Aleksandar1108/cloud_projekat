using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Features.Payments;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class PaymentRepository(
        TableServiceClient tableServiceClient,
        ITableKeyProvider<PaymentDto> keyProvider,
        ITableMapper<PaymentDto, PaymentTableEntity> mapper,
        IOptions<AzureTableOptions> options)
        : AzureTableRepository<PaymentDto, PaymentTableEntity>(
            tableServiceClient.GetTableClient(options.Value.PaymentsTable),
            keyProvider,
            mapper),
          IPaymentRepository
    {
        public async Task<PaymentDto?> GetByBillAsync(string deviceId, int year, int month, CancellationToken ct = default)
        {
            var partitionKey = $"{year:D4}-{month:D2}";
            var rowKey = Uri.EscapeDataString(deviceId);
            return await base.GetByIdAsync(partitionKey, rowKey, ct);
        }

        public async Task<PaymentDto?> GetByStripeSessionIdAsync(string stripeSessionId, CancellationToken ct = default)
        {
            // Table Storage doesn't support efficient secondary indexes; we query within recent partitions by convention.
            // We'll still provide a generic filter query (works for small datasets / sandbox).
            var filter = $"StripeSessionId eq '{stripeSessionId.Replace("'", "''")}'";
            var results = await base.QueryAsync(filter, ct);
            return results.OrderByDescending(x => x.CreatedAtUtc).FirstOrDefault();
        }

        public async Task<IReadOnlyCollection<PaymentDto>> GetAllPaidAsync(CancellationToken ct = default)
        {
            try
            {
                var paidStatus = (int)PaymentStatus.Paid;
                var filter = $"Status eq {paidStatus}";
                var results = await base.QueryAsync(filter, ct);
                return results
                    .Where(x => x.Status == PaymentStatus.Paid && x.PaidAtUtc.HasValue)
                    .OrderByDescending(x => x.PaidAtUtc)
                    .ToList();
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return Array.Empty<PaymentDto>();
            }
            catch (Exception)
            {
                return Array.Empty<PaymentDto>();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetPaidDeviceIdsForPeriodAsync(int year, int month, CancellationToken ct = default)
        {
            try
            {
                var partitionKey = $"{year:D4}-{month:D2}";
                var paidStatus = (int)PaymentStatus.Paid;
                var filter = $"PartitionKey eq '{partitionKey}' and Status eq {paidStatus}";
                var results = await base.QueryAsync(filter, ct);

                return results
                    .Where(x => x.Status == PaymentStatus.Paid)
                    .Select(x => x.DeviceId)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return Array.Empty<string>();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }
    }
}

