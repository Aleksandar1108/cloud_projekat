using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Payments;
using SmartGrid.Application.Interfaces.Repositories;
using Stripe;
using Stripe.Checkout;
using System.Globalization;
using System.IO;

namespace SmartGrid.Functions.Payments
{
    internal class StripeWebhook(
        IConfiguration configuration,
        IPaymentRepository paymentRepository,
        IStripeEventRepository stripeEventRepository,
        ILogger<StripeWebhook> logger)
    {
        [Function("StripeWebhook")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "stripe/webhook")] HttpRequest req,
            CancellationToken ct)
        {
            var webhookSecret = configuration.GetValue<string>("Stripe:WebhookSecret");
            if (string.IsNullOrWhiteSpace(webhookSecret))
            {
                logger.LogWarning("Stripe webhook secret is not configured.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            var signatureHeader = req.Headers["Stripe-Signature"].ToString();
            if (string.IsNullOrWhiteSpace(signatureHeader))
            {
                return new BadRequestObjectResult(new { error = "Missing Stripe-Signature header." });
            }

            string json;
            using (var reader = new StreamReader(req.Body))
            {
                json = await reader.ReadToEndAsync(ct);
            }

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, webhookSecret);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Stripe webhook signature verification failed.");
                return new BadRequestObjectResult(new { error = "Invalid signature." });
            }

            if (!await stripeEventRepository.TryMarkProcessedAsync(stripeEvent.Id, ct))
            {
                return new OkObjectResult(new { received = true, duplicate = true });
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                if (session is null)
                {
                    return new BadRequestObjectResult(new { error = "Invalid session payload." });
                }

                var deviceId = session.Metadata.TryGetValue("deviceId", out var d) ? d : null;
                var yearRaw = session.Metadata.TryGetValue("year", out var y) ? y : null;
                var monthRaw = session.Metadata.TryGetValue("month", out var m) ? m : null;

                PaymentDto? payment = null;

                if (!string.IsNullOrWhiteSpace(deviceId)
                    && int.TryParse(yearRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var year)
                    && int.TryParse(monthRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var month))
                {
                    payment = await paymentRepository.GetByBillAsync(deviceId, year, month, ct);

                    if (payment is null)
                    {
                        payment = new PaymentDto(
                            deviceId,
                            year,
                            month,
                            AmountMinor: session.AmountTotal ?? 0,
                            Currency: session.Currency ?? "rsd",
                            Status: PaymentStatus.Pending,
                            StripeSessionId: session.Id,
                            StripePaymentIntentId: session.PaymentIntentId,
                            CreatedAtUtc: DateTime.UtcNow,
                            PaidAtUtc: null);
                    }

                    if (payment.Status != PaymentStatus.Paid)
                    {
                        payment = payment with
                        {
                            Status = PaymentStatus.Paid,
                            StripeSessionId = session.Id,
                            StripePaymentIntentId = session.PaymentIntentId,
                            PaidAtUtc = DateTime.UtcNow
                        };

                        await paymentRepository.UpsertAsync(payment, ct);
                    }
                }
                else
                {
                    // Fallback: find by session id (works for sandbox/small datasets)
                    payment = await paymentRepository.GetByStripeSessionIdAsync(session.Id, ct);
                    if (payment is not null && payment.Status != PaymentStatus.Paid)
                    {
                        payment = payment with
                        {
                            Status = PaymentStatus.Paid,
                            StripePaymentIntentId = session.PaymentIntentId,
                            PaidAtUtc = DateTime.UtcNow
                        };
                        await paymentRepository.UpsertAsync(payment, ct);
                    }
                }

                return new OkObjectResult(new { received = true });
            }

            // Accept and ignore other events for now
            return new OkObjectResult(new { received = true, ignored = stripeEvent.Type });
        }
    }
}

