using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Billing.Queries
{
    public record GetMonthlyBillsQuery(int Year, int Month) : IRequest<Result<IReadOnlyCollection<MonthlyBillDto>>>;

    internal class GetMonthlyBillsHandler(
        IMonthlyBillRepository monthlyBillRepository,
        ILogger<GetMonthlyBillsHandler> logger)
        : IRequestHandler<GetMonthlyBillsQuery, Result<IReadOnlyCollection<MonthlyBillDto>>>
    {
        public async Task<Result<IReadOnlyCollection<MonthlyBillDto>>> Handle(GetMonthlyBillsQuery request, CancellationToken ct)
        {
            if (request.Month < 1 || request.Month > 12)
            {
                return Result<IReadOnlyCollection<MonthlyBillDto>>.Failure("Month must be in [1..12].", ErrorType.Validation);
            }

            try
            {
                var bills = await monthlyBillRepository.GetByPeriodAsync(request.Year, request.Month, ct);
                return Result<IReadOnlyCollection<MonthlyBillDto>>.Success(bills);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while loading monthly bills for {Year}-{Month}.", request.Year, request.Month);
                return Result<IReadOnlyCollection<MonthlyBillDto>>.Failure("Failed to load monthly bills.", ErrorType.Failure);
            }
        }
    }
}
