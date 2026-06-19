using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Payments;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Billing.Queries
{
    public record BillingStatsDto(
        int Year,
        int Month,
        int BillCount,
        double TotalAmount,
        int PaidCount,
        double PaidAmount,
        int UnpaidCount,
        double UnpaidAmount
    );

    public record GetBillingStatsQuery(int Year, int Month) : IRequest<Result<BillingStatsDto>>;

    internal class GetBillingStatsHandler(
        IMonthlyBillRepository monthlyBillRepository,
        IPaymentRepository paymentRepository,
        ILogger<GetBillingStatsHandler> logger)
        : IRequestHandler<GetBillingStatsQuery, Result<BillingStatsDto>>
    {
        public async Task<Result<BillingStatsDto>> Handle(GetBillingStatsQuery request, CancellationToken ct)
        {
            if (request.Month < 1 || request.Month > 12)
            {
                return Result<BillingStatsDto>.Failure("Month must be in [1..12].", ErrorType.Validation);
            }

            try
            {
                var bills = await monthlyBillRepository.GetByPeriodAsync(request.Year, request.Month, ct);
                var payments = await paymentRepository.GetByPeriodAsync(request.Year, request.Month, ct);

                var paidDeviceIds = payments
                    .Where(p => p.Status == PaymentStatus.Paid)
                    .Select(p => p.DeviceId)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var paidBills = bills.Where(b => paidDeviceIds.Contains(b.DeviceId)).ToList();
                var unpaidBills = bills.Where(b => !paidDeviceIds.Contains(b.DeviceId)).ToList();

                var stats = new BillingStatsDto(
                    request.Year,
                    request.Month,
                    bills.Count,
                    Math.Round(bills.Sum(b => b.TotalCost), 2),
                    paidBills.Count,
                    Math.Round(paidBills.Sum(b => b.TotalCost), 2),
                    unpaidBills.Count,
                    Math.Round(unpaidBills.Sum(b => b.TotalCost), 2));

                return Result<BillingStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while computing billing stats for {Year}-{Month}.", request.Year, request.Month);
                return Result<BillingStatsDto>.Failure("Failed to compute billing stats.", ErrorType.Failure);
            }
        }
    }
}
