export type ConsumptionLimitUnit = "Kwh" | "Rsd";

export interface ConsumptionLimit {
    unit: ConsumptionLimitUnit;
    limitValue: number;
    updatedAtUtc: string;
}

export interface SetConsumptionLimitDto {
    unit: ConsumptionLimitUnit;
    limitValue: number;
}
