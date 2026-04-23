using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using System.Text;

namespace SmartGrid.Application.Features.Billing.Commands
{
    public record MonthlyBillDto(
        string DeviceId,
        int Year,
        int Month,
        double TotalKwh,
        double HigherTariffKwh,
        double LowerTariffKwh,
        double GreenZoneKwh,
        double BlueZoneKwh,
        double RedZoneKwh,
        double EnergyCost,
        double FixedCosts,
        double TotalCost,
        string BillText
    );

    public record RunMonthlyBillingCommand(int Year, int Month) : IRequest<Result<IReadOnlyCollection<MonthlyBillDto>>>;

    internal sealed class RunMonthlyBillingHandler(
        ITelemetryRepository telemetryRepository,
        ITariffModelRepository tariffModelRepository,
        IMonthlyBillRepository monthlyBillRepository,
        IMonthlyBillTextStorage monthlyBillTextStorage,
        ILogger<RunMonthlyBillingHandler> logger)
        : IRequestHandler<RunMonthlyBillingCommand, Result<IReadOnlyCollection<MonthlyBillDto>>>
    {
        private const double SampleIntervalHours = 0.5;

        public async Task<Result<IReadOnlyCollection<MonthlyBillDto>>> Handle(RunMonthlyBillingCommand request, CancellationToken ct)
        {
            if (request.Month < 1 || request.Month > 12)
            {
                return Result<IReadOnlyCollection<MonthlyBillDto>>.Failure("Month must be in [1..12].", ErrorType.Validation);
            }

            var periodStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var periodEnd = periodStart.AddMonths(1);

            var telemetry = await telemetryRepository.GetByPeriodAsync(periodStart, periodEnd, ct);

            if (telemetry.Count == 0)
            {
                return Result<IReadOnlyCollection<MonthlyBillDto>>.Success(Array.Empty<MonthlyBillDto>());
            }

            var tariffModel = await tariffModelRepository.GetActiveAsync(ct);
            if (tariffModel is null)
            {
                return Result<IReadOnlyCollection<MonthlyBillDto>>.Failure(
                    "No active tariff model found. Define and activate one in SQL.",
                    ErrorType.NotFound);
            }

            var bills = new List<MonthlyBillDto>();
            var groups = telemetry.GroupBy(t => t.DeviceId.Value);

            foreach (var group in groups)
            {
                var higherKwh = 0.0;
                var lowerKwh = 0.0;

                foreach (var sample in group)
                {
                    var sampleKwh = sample.CurrentPower.Value * SampleIntervalHours;
                    if (sample.Timestamp.Hour >= 7 && sample.Timestamp.Hour < 23)
                    {
                        higherKwh += sampleKwh;
                    }
                    else
                    {
                        lowerKwh += sampleKwh;
                    }
                }

                var bill = BuildBill(group.Key, request.Year, request.Month, higherKwh, lowerKwh, tariffModel);

                await monthlyBillTextStorage.SaveAsync(new FileData<MonthlyBillTextMetadata>
                {
                    Content = Encoding.UTF8.GetBytes(bill.BillText),
                    Metadata = new MonthlyBillTextMetadata
                    {
                        Year = request.Year,
                        Month = request.Month,
                        DeviceId = group.Key
                    }
                }, ct);

                await monthlyBillRepository.SaveOrUpdateAsync(bill, ct);

                bills.Add(bill);
            }

            logger.LogInformation("Monthly billing generated for {Count} devices. Period: {Year}-{Month}.", bills.Count, request.Year, request.Month);

            return Result<IReadOnlyCollection<MonthlyBillDto>>.Success(bills.OrderBy(b => b.DeviceId).ToList());
        }

        private static MonthlyBillDto BuildBill(
            string deviceId,
            int year,
            int month,
            double higherKwh,
            double lowerKwh,
            TariffModelSettings tariffModel)
        {
            var totalKwh = higherKwh + lowerKwh;
            var higherCoef = totalKwh <= 0 ? 0 : higherKwh / totalKwh;
            var lowerCoef = totalKwh <= 0 ? 0 : lowerKwh / totalKwh;

            var greenTotal = Math.Min(totalKwh, 350);
            var blueTotal = Math.Max(0, Math.Min(totalKwh - 350, 850));
            var redTotal = Math.Max(0, totalKwh - 1200);

            var greenVt = greenTotal * higherCoef;
            var greenNt = greenTotal * lowerCoef;
            var blueVt = blueTotal * higherCoef;
            var blueNt = blueTotal * lowerCoef;
            var redVt = redTotal * higherCoef;
            var redNt = redTotal * lowerCoef;

            var greenAmount = (greenVt * tariffModel.GreenZoneVtPrice) + (greenNt * tariffModel.GreenZoneNtPrice);
            var blueAmount = (blueVt * tariffModel.BlueZoneVtPrice) + (blueNt * tariffModel.BlueZoneNtPrice);
            var redAmount = (redVt * tariffModel.RedZoneVtPrice) + (redNt * tariffModel.RedZoneNtPrice);

            var energyCost = greenAmount + blueAmount + redAmount;
            var fixedCosts = (tariffModel.ApprovedPowerKw * tariffModel.NetworkCostPerKw) + tariffModel.SupplierCost;
            var totalCost = energyCost + fixedCosts;

            var text = new StringBuilder()
                .AppendLine("SMART GRID - MESECNI RACUN")
                .AppendLine($"Uredjaj: {deviceId}")
                .AppendLine($"Period: {year:D4}-{month:D2}")
                .AppendLine($"VT (kWh): {higherKwh:F2}")
                .AppendLine($"NT (kWh): {lowerKwh:F2}")
                .AppendLine($"Ukupno (kWh): {totalKwh:F2}")
                .AppendLine($"Zona zelena (kWh): {greenTotal:F2}")
                .AppendLine($"Zona plava (kWh): {blueTotal:F2}")
                .AppendLine($"Zona crvena (kWh): {redTotal:F2}")
                .AppendLine($"Cena energije (RSD): {energyCost:F2}")
                .AppendLine($"Fiksni troskovi (RSD): {fixedCosts:F2}")
                .AppendLine($"UKUPNO ZA UPLATU (RSD): {totalCost:F2}")
                .ToString();

            return new MonthlyBillDto(
                deviceId,
                year,
                month,
                Math.Round(totalKwh, 2),
                Math.Round(higherKwh, 2),
                Math.Round(lowerKwh, 2),
                Math.Round(greenTotal, 2),
                Math.Round(blueTotal, 2),
                Math.Round(redTotal, 2),
                Math.Round(energyCost, 2),
                Math.Round(fixedCosts, 2),
                Math.Round(totalCost, 2),
                text);
        }
    }
}
