export interface DeviceStatus {
    deviceId: string;
    deviceType: string;
    currentPower: number;
    loadPercentage: number;
    isOnline: boolean;
    isUnderperforming: boolean;
    isOverloaded: boolean;
    currentFirmwareVersion: string;
    targetFirmwareVersion: string | null;
    updateStatus: string;
    label?: string | null;
}
