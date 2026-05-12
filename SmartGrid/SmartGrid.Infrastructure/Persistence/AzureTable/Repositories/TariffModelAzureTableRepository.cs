using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories;

internal sealed class TariffModelAzureTableRepository(
    TableServiceClient tableServiceClient,
    IOptions<AzureTableOptions> options) : ITariffModelRepository
{
    private const string Partition = "TariffModels";

    public async Task<TariffModelSettings?> GetActiveAsync(CancellationToken ct = default)
    {
        var client = tableServiceClient.GetTableClient(options.Value.TariffModelsTable);
        TariffModelTableEntity? best = null;

        try
        {
            await foreach (var entity in client.QueryAsync<TariffModelTableEntity>($"PartitionKey eq '{Partition}' and IsActive eq true", cancellationToken: ct))
            {
                if (best is null || entity.CreatedAtUtc > best.CreatedAtUtc)
                    best = entity;
            }
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }

        if (best is null)
            return null;

        return new TariffModelSettings(
            best.GreenZoneVtPrice,
            best.GreenZoneNtPrice,
            best.BlueZoneVtPrice,
            best.BlueZoneNtPrice,
            best.RedZoneVtPrice,
            best.RedZoneNtPrice,
            best.NetworkCostPerKw,
            best.SupplierCost,
            best.ApprovedPowerKw);
    }
}
