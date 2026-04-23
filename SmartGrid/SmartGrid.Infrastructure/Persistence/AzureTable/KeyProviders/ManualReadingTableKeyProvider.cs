using SmartGrid.Application.Features.ManualReadings;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class ManualReadingTableKeyProvider : ITableKeyProvider<ManualReadingDto>
    {
        public string GetPartitionKey(ManualReadingDto model)
        {
            return model.Status.ToString();
        }

        public string GetRowKey(ManualReadingDto model)
        {
            return model.Id.ToString();
        }
    }
}
