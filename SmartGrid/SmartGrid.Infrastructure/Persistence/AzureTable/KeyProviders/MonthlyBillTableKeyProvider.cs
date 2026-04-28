using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class MonthlyBillTableKeyProvider : ITableKeyProvider<MonthlyBillDto>
    {
        public string GetPartitionKey(MonthlyBillDto model)
        {
            return $"{model.Year:D4}-{model.Month:D2}";
        }

        public string GetRowKey(MonthlyBillDto model)
        {
            return Uri.EscapeDataString(model.DeviceId);
        }
    }
}
