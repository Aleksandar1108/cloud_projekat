using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Payments;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.WebApi.DTOs;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController(
        IMonthlyBillRepository monthlyBillRepository,
        IPaymentRepository paymentRepository,
        IPaymentCheckoutService checkoutService)
        : ControllerBase
    {
        [HttpPost("checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequestDto request, CancellationToken ct)
        {
            if (request.Month < 1 || request.Month > 12)
            {
                return BadRequest(new { message = "Month must be in [1..12]." });
            }

            var bill = await monthlyBillRepository.GetAsync(request.Year, request.Month, request.DeviceId, ct);
            if (bill is null)
            {
                return NotFound(new { message = "Monthly bill not found." });
            }

            var existing = await paymentRepository.GetByBillAsync(request.DeviceId, request.Year, request.Month, ct);
            if (existing?.Status == PaymentStatus.Paid)
            {
                return Conflict(new { message = "Bill is already paid." });
            }

            var (sessionId, url) = await checkoutService.CreateCheckoutSessionAsync(bill, ct);

            var amountMinor = (long)Math.Round(bill.TotalCost * 100.0, MidpointRounding.AwayFromZero);
            var payment = new PaymentDto(
                request.DeviceId,
                request.Year,
                request.Month,
                amountMinor,
                "rsd",
                PaymentStatus.Pending,
                sessionId,
                StripePaymentIntentId: null,
                CreatedAtUtc: DateTime.UtcNow,
                PaidAtUtc: null
            );

            await paymentRepository.UpsertAsync(payment, ct);

            return Ok(new
            {
                sessionId,
                url
            });
        }
    }
}

