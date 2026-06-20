using SmartGrid.Application.Features.Payments;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<PaymentDto?> GetByBillAsync(string deviceId, int year, int month, CancellationToken ct = default);
        Task<PaymentDto?> GetByStripeSessionIdAsync(string stripeSessionId, CancellationToken ct = default);
        Task UpsertAsync(PaymentDto payment, CancellationToken ct = default);
    }
}

