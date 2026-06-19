export interface TariffModel {
    id: number;
    name: string;
    isActive: boolean;
    createdAt: string;
    greenZoneVtPrice: number;
    greenZoneNtPrice: number;
    blueZoneVtPrice: number;
    blueZoneNtPrice: number;
    redZoneVtPrice: number;
    redZoneNtPrice: number;
    networkCostPerKw: number;
    supplierCost: number;
    approvedPowerKw: number;
    greenZoneLimitKwh: number;
    blueZoneLimitKwh: number;
}

export type TariffModelInput = Omit<TariffModel, "id" | "createdAt">;
