using Microsoft.Extensions.Options;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Interfaces;
using SmartGrid.Infrastructure.Common.Options;
using Stripe;
using Stripe.Checkout;

namespace SmartGrid.Infrastructure.Services
{
    internal class StripeCheckoutService(IOptions<StripeOptions> options) : IPaymentCheckoutService
    {
        private readonly StripeOptions _options = options.Value;

        public async Task<(string SessionId, string Url)> CreateCheckoutSessionAsync(MonthlyBillDto bill, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_options.SecretKey))
            {
                throw new InvalidOperationException("Stripe:SecretKey is not configured.");
            }
            if (string.IsNullOrWhiteSpace(_options.SuccessUrl) || string.IsNullOrWhiteSpace(_options.CancelUrl))
            {
                throw new InvalidOperationException("Stripe:SuccessUrl / Stripe:CancelUrl are not configured.");
            }

            StripeConfiguration.ApiKey = _options.SecretKey;

            var amountMinor = (long)Math.Round(bill.TotalCost * 100.0, MidpointRounding.AwayFromZero);
            if (amountMinor <= 0)
            {
                throw new InvalidOperationException("Bill amount must be greater than 0.");
            }

            var sessionOptions = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = _options.SuccessUrl,
                CancelUrl = _options.CancelUrl,
                ClientReferenceId = $"{bill.Year:D4}-{bill.Month:D2}:{bill.DeviceId}",
                Metadata = new Dictionary<string, string>
                {
                    ["deviceId"] = bill.DeviceId,
                    ["year"] = bill.Year.ToString(),
                    ["month"] = bill.Month.ToString()
                },
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = _options.Currency,
                            UnitAmount = amountMinor,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"SmartGrid racun {bill.Year:D4}-{bill.Month:D2}",
                                Description = $"Uredjaj: {bill.DeviceId}"
                            }
                        }
                    }
                ]
            };

            var service = new SessionService();
            var session = await service.CreateAsync(sessionOptions, requestOptions: null, cancellationToken: ct);

            if (string.IsNullOrWhiteSpace(session.Url))
            {
                throw new InvalidOperationException("Stripe session URL is missing.");
            }

            return (session.Id, session.Url);
        }
    }
}

