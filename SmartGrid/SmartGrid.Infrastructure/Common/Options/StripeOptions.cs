namespace SmartGrid.Infrastructure.Common.Options
{
    internal class StripeOptions
    {
        public string SecretKey { get; init; } = string.Empty;
        public string WebhookSecret { get; init; } = string.Empty;
        public string Currency { get; init; } = "rsd";
        public string SuccessUrl { get; init; } = string.Empty;
        public string CancelUrl { get; init; } = string.Empty;
    }
}

