using SmartGrid.Application.Interfaces.Repositories;

namespace SmartGrid.Application.Common
{
    internal static class ZoneConsumptionCalculator
    {
        public static (double GreenTotal, double BlueTotal, double RedTotal) SplitIntoZones(
            double totalKwh,
            TariffModelSettings tariff)
        {
            var greenMax = tariff.GreenZoneMaxKwh;
            var blueMax = tariff.BlueZoneMaxKwh;

            var greenTotal = Math.Min(totalKwh, greenMax);
            var blueTotal = Math.Max(0, Math.Min(totalKwh - greenMax, blueMax - greenMax));
            var redTotal = Math.Max(0, totalKwh - blueMax);

            return (greenTotal, blueTotal, redTotal);
        }
    }
}
