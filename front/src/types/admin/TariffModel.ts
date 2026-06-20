export type ZonePrices = {
    vtPriceRsd: number;
    ntPriceRsd: number;
};

export type TariffModel = {
    id: string;
    name: string;
    effectiveFrom: string;
    zoneThresholds: {
        greenMaxKwh: number;
        blueMaxKwh: number;
    };
    zonePrices: {
        green: ZonePrices;
        blue: ZonePrices;
        red: ZonePrices;
    };
    fixedCosts: {
        billingPowerFeeRsd: number;
        supplierCostRsd: number;
    };
    updatedAtUtc: string;
};
