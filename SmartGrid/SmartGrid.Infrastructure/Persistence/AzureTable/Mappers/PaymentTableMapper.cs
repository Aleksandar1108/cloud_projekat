using SmartGrid.Application.Features.Payments;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Mappers
{
    internal class PaymentTableMapper : ITableMapper<PaymentDto, PaymentTableEntity>
    {
        public PaymentTableEntity ToEntity(PaymentDto domain)
        {
            return new PaymentTableEntity
            {
                Year = domain.Year,
                Month = domain.Month,
                AmountMinor = domain.AmountMinor,
                Currency = domain.Currency,
                Status = domain.Status,
                StripeSessionId = domain.StripeSessionId,
                StripePaymentIntentId = domain.StripePaymentIntentId,
                CreatedAtUtc = domain.CreatedAtUtc,
                PaidAtUtc = domain.PaidAtUtc
            };
        }

        public PaymentDto? ToDomain(PaymentTableEntity entity)
        {
            var deviceId = Uri.UnescapeDataString(entity.RowKey);

            return new PaymentDto(
                deviceId,
                entity.Year,
                entity.Month,
                entity.AmountMinor,
                entity.Currency,
                entity.Status,
                entity.StripeSessionId,
                entity.StripePaymentIntentId,
                entity.CreatedAtUtc,
                entity.PaidAtUtc
            );
        }
    }
}

