import type { SmartMeterTelemetryAnalytics } from "../../types/telemetry/TelemetryAnalytics";

export const CRITICAL_VOLTAGE = 190;

export function hasCriticalAlert(meter: SmartMeterTelemetryAnalytics): boolean {
    const voltage = meter.status.currentVoltage;
    const isLowVoltage = typeof voltage === "number" && voltage < CRITICAL_VOLTAGE;
    const isOffline = meter.status.isOnline === false;
    return isLowVoltage || isOffline;
}
