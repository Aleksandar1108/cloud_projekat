using SmartGrid.Application.Features.Billing.Commands;

namespace SmartGrid.Application.Interfaces
{
    public interface IPaymentCheckoutService
    {
        Task<(string SessionId, string Url)> CreateCheckoutSessionAsync(
            MonthlyBillDto bill,
            CancellationToken ct = default);
    }
}

