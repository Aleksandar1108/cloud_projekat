using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories;

internal sealed class ConsumptionLimitAzureTableRepository(
    TableServiceClient tableServiceClient,
    IOptions<AzureTableOptions> options) : IConsumptionLimitRepository
{
    private TableClient LimitsClient => tableServiceClient.GetTableClient(options.Value.ConsumptionLimitsTable);
    private TableClient NotifiedClient => tableServiceClient.GetTableClient(options.Value.ConsumptionLimitNotifiedTable);

    public async Task<ConsumptionLimitSetting?> GetByDeviceIdAsync(Guid deviceId, CancellationToken ct = default)
    {
        var rk = OdataEscape(deviceId.ToString("D"));
        try
        {
            await foreach (var entity in LimitsClient.QueryAsync<ConsumptionLimitTableEntity>($"RowKey eq '{rk}'", cancellationToken: ct))
            {
                if (!Guid.TryParse(entity.PartitionKey, out var userId))
                    continue;

                return new ConsumptionLimitSetting
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    LimitKwh = (decimal)entity.LimitKwh,
                    LimitRsd = entity.LimitRsd is null ? null : (decimal)entity.LimitRsd.Value
                };
            }
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }

        return null;
    }

    public async Task<IReadOnlyList<ConsumptionLimitSetting>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var pk = OdataEscape(userId.ToString("D"));
        var list = new List<ConsumptionLimitSetting>();
        try
        {
            await foreach (var entity in LimitsClient.QueryAsync<ConsumptionLimitTableEntity>($"PartitionKey eq '{pk}'", cancellationToken: ct))
            {
                if (!Guid.TryParse(entity.RowKey, out var deviceId))
                    continue;

                list.Add(new ConsumptionLimitSetting
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    LimitKwh = (decimal)entity.LimitKwh,
                    LimitRsd = entity.LimitRsd is null ? null : (decimal)entity.LimitRsd.Value
                });
            }
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return list;
        }

        return list;
    }

    public async Task UpsertAsync(ConsumptionLimitSetting row, CancellationToken ct = default)
    {
        await LimitsClient.CreateIfNotExistsAsync(ct);

        var entity = new ConsumptionLimitTableEntity
        {
            PartitionKey = row.UserId.ToString("D"),
            RowKey = row.DeviceId.ToString("D"),
            LimitKwh = (double)row.LimitKwh,
            LimitRsd = row.LimitRsd is null ? null : (double)row.LimitRsd.Value,
            ETag = ETag.All
        };

        await LimitsClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
    }

    public async Task<bool> WasLimitEmailSentAsync(Guid userId, Guid deviceId, int yearMonth, CancellationToken ct = default)
    {
        var pk = userId.ToString("D");
        var rk = NotifiedRowKey(deviceId, yearMonth);
        try
        {
            var response = await NotifiedClient.GetEntityIfExistsAsync<ConsumptionLimitNotifiedTableEntity>(pk, rk, cancellationToken: ct);
            return response.HasValue && response.Value is not null;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    public async Task RecordLimitEmailSentAsync(Guid userId, Guid deviceId, int yearMonth, CancellationToken ct = default)
    {
        if (await WasLimitEmailSentAsync(userId, deviceId, yearMonth, ct))
            return;

        await NotifiedClient.CreateIfNotExistsAsync(ct);

        var entity = new ConsumptionLimitNotifiedTableEntity
        {
            PartitionKey = userId.ToString("D"),
            RowKey = NotifiedRowKey(deviceId, yearMonth),
            NotifiedAtUtc = DateTime.UtcNow,
            ETag = ETag.All
        };

        await NotifiedClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
    }

    private static string NotifiedRowKey(Guid deviceId, int yearMonth) => $"{deviceId:D}_{yearMonth}";

    private static string OdataEscape(string value) => value.Replace("'", "''");
}
