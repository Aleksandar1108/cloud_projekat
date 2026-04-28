using SmartGrid.Application.Features.Payments;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class PaymentTableEntity : BaseTableEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long AmountMinor { get; set; }
        public string Currency { get; set; } = "rsd";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }
    }
}

