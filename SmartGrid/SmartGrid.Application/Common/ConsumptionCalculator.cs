using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Common
{
    internal static class ConsumptionCalculator
    {
        private const double SampleIntervalHours = 0.5;

        public static double CalculateKwh(IEnumerable<Telemetry> samples)
        {
            return samples.Sum(s => s.CurrentPower.Value * SampleIntervalHours);
        }

        public static double EstimateEnergyCost(double totalKwh, TariffModelSettings tariff)
        {
            if (totalKwh <= 0)
            {
                return 0;
            }

            var higherCoef = 0.65;
            var lowerCoef = 0.35;

            var greenTotal = Math.Min(totalKwh, 350);
            var blueTotal = Math.Max(0, Math.Min(totalKwh - 350, 850));
            var redTotal = Math.Max(0, totalKwh - 1200);

            var greenVt = greenTotal * higherCoef;
            var greenNt = greenTotal * lowerCoef;
            var blueVt = blueTotal * higherCoef;
            var blueNt = blueTotal * lowerCoef;
            var redVt = redTotal * higherCoef;
            var redNt = redTotal * lowerCoef;

            return (greenVt * tariff.GreenZoneVtPrice) + (greenNt * tariff.GreenZoneNtPrice)
                + (blueVt * tariff.BlueZoneVtPrice) + (blueNt * tariff.BlueZoneNtPrice)
                + (redVt * tariff.RedZoneVtPrice) + (redNt * tariff.RedZoneNtPrice);
        }
    }
}
