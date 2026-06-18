export interface DailyTariffConsumption {
    dayUtc: string;
    higherTariffKwh: number;
    lowerTariffKwh: number;
}

export interface TelemetryTrendPoint {
    timestampUtc: string;
    loadPercentage: number;
    currentPowerKw: number;
    voltage?: number | null;
}

export interface TelemetryStatusCard {
    currentTariff: string;
    currentPowerKw: number;
    loadPercentage: number;
    currentVoltage?: number | null;
    isOnline: boolean;
    lastHeartbeatUtc?: string | null;
}

export interface SmartMeterTelemetryAnalytics {
    smartMeterId: string;
    label: string;
    deviceUuid?: string | null;
    dailyConsumption: DailyTariffConsumption[];
    trend: TelemetryTrendPoint[];
    status: TelemetryStatusCard;
}

export interface PropertyTelemetryAnalytics {
    propertyId: string;
    generatedAtUtc: string;
    meters: SmartMeterTelemetryAnalytics[];
}

export interface DeviceStatusSignalRDto {
    deviceId: string;
    currentPower: number;
    loadPercentage: number;
    isOnline: boolean;
    voltage?: number | null;
    currentVoltage?: number | null;
}
