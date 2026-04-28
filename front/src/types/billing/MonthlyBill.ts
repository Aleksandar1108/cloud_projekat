export type MonthlyBill = {
    deviceId: string;
    year: number;
    month: number;
    totalKwh: number;
    higherTariffKwh: number;
    lowerTariffKwh: number;
    greenZoneKwh: number;
    blueZoneKwh: number;
    redZoneKwh: number;
    energyCost: number;
    fixedCosts: number;
    totalCost: number;
    billText: string;
};
