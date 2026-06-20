using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces;
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
        IManualReadingRepository manualReadingRepository,
        ITariffModelRepository tariffModelRepository,
        IMonthlyBillRepository monthlyBillRepository,
        IMonthlyBillTextStorage monthlyBillTextStorage,
        IUserRepository userRepository,
        IEmailService emailService,
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
            var processedManualReadings = await manualReadingRepository.GetProcessedByPeriodAsync(periodStart, periodEnd, ct);

            if (telemetry.Count == 0 && processedManualReadings.Count == 0)
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
            var deviceIds = telemetry.Select(t => t.DeviceId.Value)
                .Concat(processedManualReadings.Select(m => m.DeviceId))
                .Distinct()
                .ToList();

            foreach (var deviceId in deviceIds)
            {
                var higherKwh = 0.0;
                var lowerKwh = 0.0;

                foreach (var sample in telemetry.Where(t => t.DeviceId.Value == deviceId))
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

                var processedForDevice = processedManualReadings.Where(x => x.DeviceId == deviceId);
                foreach (var manual in processedForDevice)
                {
                    if (manual.ReadingAtUtc.Hour >= 7 && manual.ReadingAtUtc.Hour < 23)
                    {
                        higherKwh += manual.ReadingKwh;
                    }
                    else
                    {
                        lowerKwh += manual.ReadingKwh;
                    }
                }

                var bill = BuildBill(deviceId, request.Year, request.Month, higherKwh, lowerKwh, tariffModel);

                await monthlyBillTextStorage.SaveAsync(new FileData<MonthlyBillTextMetadata>
                {
                    Content = GeneratePdfBytes(bill),
                    Metadata = new MonthlyBillTextMetadata
                    {
                        Year = request.Year,
                        Month = request.Month,
                        DeviceId = deviceId
                    }
                }, ct);

                await monthlyBillRepository.SaveOrUpdateAsync(bill, ct);

                bills.Add(bill);
            }

            logger.LogInformation("Monthly billing generated for {Count} devices. Period: {Year}-{Month}.", bills.Count, request.Year, request.Month);

            await SendMonthlyBillingEmailSummaryAsync(request.Year, request.Month, bills, userRepository, emailService, logger, ct);

            return Result<IReadOnlyCollection<MonthlyBillDto>>.Success(bills.OrderBy(b => b.DeviceId).ToList());
        }

        private static async Task SendMonthlyBillingEmailSummaryAsync(
            int year,
            int month,
            IReadOnlyCollection<MonthlyBillDto> bills,
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<RunMonthlyBillingHandler> logger,
            CancellationToken ct)
        {
            try
            {
                var users = await userRepository.GetAllAsync(ct);
                var recipients = users
                    .Where(u => u.Role == UserRole.User)
                    .Select(u => u.Email.Value)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (recipients.Count == 0)
                {
                    logger.LogInformation("No consumer recipients found for monthly billing email.");
                    return;
                }

                var totalAmount = bills.Sum(x => x.TotalCost);
                var body = new StringBuilder()
                    .AppendLine("Postovani,")
                    .AppendLine()
                    .AppendLine($"Automatizovani mesecni obracun je zavrsen za period {year:D4}-{month:D2}.")
                    .AppendLine($"Generisano racuna: {bills.Count}")
                    .AppendLine($"Ukupan obracunat iznos (RSD): {totalAmount:F2}")
                    .AppendLine()
                    .AppendLine("Detalji racuna su dostupni u SmartGrid aplikaciji.")
                    .ToString();

                foreach (var recipient in recipients)
                {
                    await emailService.SendEmailAsync(
                        recipient,
                        $"SmartGrid - Mesecni obracun {year:D4}-{month:D2}",
                        body,
                        ct);
                }

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Monthly billing was generated, but email sending failed.");
            }
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

        private static byte[] GeneratePdfBytes(MonthlyBillDto bill)
        {
            var lines = new[]
            {
                "SMART GRID - MESECNI RACUN",
                $"Uredjaj: {bill.DeviceId}",
                $"Period: {bill.Year:D4}-{bill.Month:D2}",
                $"VT (kWh): {bill.HigherTariffKwh:F2}",
                $"NT (kWh): {bill.LowerTariffKwh:F2}",
                $"Ukupno (kWh): {bill.TotalKwh:F2}",
                $"Zona zelena (kWh): {bill.GreenZoneKwh:F2}",
                $"Zona plava (kWh): {bill.BlueZoneKwh:F2}",
                $"Zona crvena (kWh): {bill.RedZoneKwh:F2}",
                $"Cena energije (RSD): {bill.EnergyCost:F2}",
                $"Fiksni troskovi (RSD): {bill.FixedCosts:F2}",
                $"UKUPNO ZA UPLATU (RSD): {bill.TotalCost:F2}"
            };

            string Escape(string value) => value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
            var textOperations = new StringBuilder("BT /F1 12 Tf 50 770 Td 16 TL ");
            foreach (var line in lines)
            {
                textOperations.Append($"({Escape(line)}) Tj T* ");
            }
            textOperations.Append("ET");

            var contentStream = textOperations.ToString();
            var contentLength = Encoding.ASCII.GetByteCount(contentStream);

            var pdf = new StringBuilder();
            var offsets = new List<int>();

            void AppendObject(int id, string body)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
                pdf.Append($"{id} 0 obj\n{body}\nendobj\n");
            }

            pdf.Append("%PDF-1.4\n");
            AppendObject(1, "<< /Type /Catalog /Pages 2 0 R >>");
            AppendObject(2, "<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
            AppendObject(3, "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>");
            AppendObject(4, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            AppendObject(5, $"<< /Length {contentLength} >>\nstream\n{contentStream}\nendstream");

            var xrefStart = Encoding.ASCII.GetByteCount(pdf.ToString());
            pdf.Append("xref\n0 6\n");
            pdf.Append("0000000000 65535 f \n");
            foreach (var offset in offsets)
            {
                pdf.Append($"{offset:D10} 00000 n \n");
            }
            pdf.Append("trailer\n<< /Size 6 /Root 1 0 R >>\n");
            pdf.Append($"startxref\n{xrefStart}\n%%EOF");

            return Encoding.ASCII.GetBytes(pdf.ToString());
        }
    }
}
