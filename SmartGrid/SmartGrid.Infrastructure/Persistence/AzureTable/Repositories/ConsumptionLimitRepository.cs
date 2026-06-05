using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Enums;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class ConsumptionLimitRepository(
        TableServiceClient tableServiceClient,
        IOptions<AzureTableOptions> options) : IConsumptionLimitRepository
    {
        private readonly TableClient _tableClient = tableServiceClient.GetTableClient(options.Value.ConsumptionLimitsTable);

        public async Task<ConsumptionLimitSettings?> GetAsync(Guid userId, Guid smartMeterId, CancellationToken ct = default)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<ConsumptionLimitTableEntity>(
                    userId.ToString(),
                    smartMeterId.ToString(),
                    cancellationToken: ct);

                var entity = response.Value;
                if (!Enum.TryParse<ConsumptionLimitUnit>(entity.Unit, out var unit))
                {
                    return null;
                }

                return new ConsumptionLimitSettings(
                    userId,
                    smartMeterId,
                    unit,
                    entity.LimitValue,
                    entity.UpdatedAtUtc);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task SaveAsync(ConsumptionLimitSettings settings, CancellationToken ct = default)
        {
            var entity = new ConsumptionLimitTableEntity
            {
                PartitionKey = settings.UserId.ToString(),
                RowKey = settings.SmartMeterId.ToString(),
                Unit = settings.Unit.ToString(),
                LimitValue = settings.LimitValue,
                UpdatedAtUtc = settings.UpdatedAtUtc
            };

            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
        }

        public async Task DeleteAsync(Guid userId, Guid smartMeterId, CancellationToken ct = default)
        {
            try
            {
                await _tableClient.DeleteEntityAsync(userId.ToString(), smartMeterId.ToString(), cancellationToken: ct);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
            }
        }
    }
}
