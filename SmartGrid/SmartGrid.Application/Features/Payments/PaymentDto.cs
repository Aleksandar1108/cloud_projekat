namespace SmartGrid.Application.Features.Payments
{
    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2,
        Canceled = 3
    }

    public record PaymentDto(
        string DeviceId,
        int Year,
        int Month,
        long AmountMinor,
        string Currency,
        PaymentStatus Status,
        string? StripeSessionId,
        string? StripePaymentIntentId,
        DateTime CreatedAtUtc,
        DateTime? PaidAtUtc
    );
}

