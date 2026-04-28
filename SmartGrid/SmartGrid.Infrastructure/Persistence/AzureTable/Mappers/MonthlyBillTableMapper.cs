using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Mappers
{
    internal class MonthlyBillTableMapper : ITableMapper<MonthlyBillDto, MonthlyBillTableEntity>
    {
        public MonthlyBillTableEntity ToEntity(MonthlyBillDto domain)
        {
            return new MonthlyBillTableEntity
            {
                Year = domain.Year,
                Month = domain.Month,
                TotalKwh = domain.TotalKwh,
                HigherTariffKwh = domain.HigherTariffKwh,
                LowerTariffKwh = domain.LowerTariffKwh,
                GreenZoneKwh = domain.GreenZoneKwh,
                BlueZoneKwh = domain.BlueZoneKwh,
                RedZoneKwh = domain.RedZoneKwh,
                EnergyCost = domain.EnergyCost,
                FixedCosts = domain.FixedCosts,
                TotalCost = domain.TotalCost,
                BillText = domain.BillText,
                GeneratedAtUtc = DateTime.UtcNow
            };
        }

        public MonthlyBillDto? ToDomain(MonthlyBillTableEntity entity)
        {
            var deviceId = Uri.UnescapeDataString(entity.RowKey);

            return new MonthlyBillDto(
                deviceId,
                entity.Year,
                entity.Month,
                entity.TotalKwh,
                entity.HigherTariffKwh,
                entity.LowerTariffKwh,
                entity.GreenZoneKwh,
                entity.BlueZoneKwh,
                entity.RedZoneKwh,
                entity.EnergyCost,
                entity.FixedCosts,
                entity.TotalCost,
                entity.BillText
            );
        }
    }
}
