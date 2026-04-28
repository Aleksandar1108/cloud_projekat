using SmartGrid.Application.Features.Payments;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class PaymentTableKeyProvider : ITableKeyProvider<PaymentDto>
    {
        public string GetPartitionKey(PaymentDto model)
        {
            return $"{model.Year:D4}-{model.Month:D2}";
        }

        public string GetRowKey(PaymentDto model)
        {
            return Uri.EscapeDataString(model.DeviceId);
        }
    }
}

