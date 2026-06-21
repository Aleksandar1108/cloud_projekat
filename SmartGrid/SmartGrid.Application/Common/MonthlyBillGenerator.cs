using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Interfaces.Repositories;
using System.Text;

namespace SmartGrid.Application.Common
{
    internal static class MonthlyBillGenerator
    {
        public static MonthlyBillDto BuildBill(
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

            var (greenTotal, blueTotal, redTotal) = ZoneConsumptionCalculator.SplitIntoZones(totalKwh, tariffModel);

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

        public static byte[] GeneratePdfBytes(MonthlyBillDto bill)
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
