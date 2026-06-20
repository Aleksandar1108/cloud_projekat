using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Admin;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Admin.Billing
{
    public record GetBillingRunsQuery() : IRequest<Result<IReadOnlyCollection<BillingRunDto>>>;

    internal sealed class GetBillingRunsHandler(
        IMonthlyBillRepository monthlyBillRepository,
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        ILogger<GetBillingRunsHandler> logger)
        : IRequestHandler<GetBillingRunsQuery, Result<IReadOnlyCollection<BillingRunDto>>>
    {
        public async Task<Result<IReadOnlyCollection<BillingRunDto>>> Handle(
            GetBillingRunsQuery request,
            CancellationToken ct)
        {
            try
            {
                var periods = await monthlyBillRepository.GetPeriodSummariesAsync(ct);
                var meters = await smartMeterRepository.GetAllPairedAsync(ct);
                var meterByDevice = meters
                    .Where(x => !string.IsNullOrWhiteSpace(x.DeviceUUID))
                    .ToDictionary(x => x.DeviceUUID!, StringComparer.OrdinalIgnoreCase);
                var users = (await userRepository.GetAllAsync(ct)).ToDictionary(x => x.Id.Value);

                var runs = new List<BillingRunDto>();

                foreach (var period in periods)
                {
                    var bills = await monthlyBillRepository.GetByPeriodAsync(period.Year, period.Month, ct);
                    var adminBills = new List<AdminGeneratedBillDto>();
                    var index = 0;

                    foreach (var bill in bills)
                    {
                        index++;
                        meterByDevice.TryGetValue(bill.DeviceId, out var meter);
                        var ownerEmail = "N/A";
                        var ownerName = "Nepoznat korisnik";

                        if (meter is not null)
                        {
                            var property = await propertyRepository.GetByIdAsync(meter.PropertyId, ct);
                            if (property is not null && users.TryGetValue(property.UserId, out var owner))
                            {
                                ownerEmail = owner.Email.Value;
                                ownerName = AdminOwnerResolver.ResolveName(owner);
                            }
                        }

                        adminBills.Add(new AdminGeneratedBillDto(
                            $"INV-{bill.Year:D4}-{bill.Month:D2}-{index:D4}",
                            bill.DeviceId,
                            ownerName,
                            ownerEmail,
                            bill.Year,
                            bill.Month,
                            bill.HigherTariffKwh,
                            bill.LowerTariffKwh,
                            bill.TotalKwh,
                            bill.GreenZoneKwh,
                            bill.BlueZoneKwh,
                            bill.RedZoneKwh,
                            bill.EnergyCost,
                            bill.FixedCosts,
                            bill.TotalCost,
                            bill.BillText,
                            true));
                    }

                    var completedAt = period.LastGeneratedAtUtc ?? DateTime.UtcNow;
                    runs.Add(new BillingRunDto(
                        $"run-{period.Year}-{period.Month}",
                        period.Year,
                        period.Month,
                        completedAt.ToString("o"),
                        completedAt.ToString("o"),
                        "Completed",
                        adminBills.Count,
                        adminBills.Count,
                        adminBills.Count,
                        adminBills));
                }

                return Result<IReadOnlyCollection<BillingRunDto>>.Success(runs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load billing runs.");
                return Result<IReadOnlyCollection<BillingRunDto>>.Failure(
                    "Failed to load billing runs.",
                    ErrorType.Failure);
            }
        }
    }

    public record GetBillingDeliveryStatsQuery() : IRequest<Result<BillingDeliveryStatsDto>>;

    internal sealed class GetBillingDeliveryStatsHandler(
        IMonthlyBillRepository monthlyBillRepository,
        ILogger<GetBillingDeliveryStatsHandler> logger)
        : IRequestHandler<GetBillingDeliveryStatsQuery, Result<BillingDeliveryStatsDto>>
    {
        public async Task<Result<BillingDeliveryStatsDto>> Handle(
            GetBillingDeliveryStatsQuery request,
            CancellationToken ct)
        {
            try
            {
                var periods = await monthlyBillRepository.GetPeriodSummariesAsync(ct);
                var totalGenerated = await monthlyBillRepository.GetTotalBillCountAsync(ct);
                var lastRun = periods.FirstOrDefault()?.LastGeneratedAtUtc;

                return Result<BillingDeliveryStatsDto>.Success(new BillingDeliveryStatsDto(
                    totalGenerated,
                    totalGenerated,
                    0,
                    lastRun?.ToString("o"),
                    totalGenerated > 0 ? 100 : 0));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load billing delivery stats.");
                return Result<BillingDeliveryStatsDto>.Failure(
                    "Failed to load billing delivery stats.",
                    ErrorType.Failure);
            }
        }
    }
}
