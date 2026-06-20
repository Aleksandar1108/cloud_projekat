export type MeterConnectionStatus = "Online" | "Offline";

export type MeterNetworkStatus = {
    deviceId: string;
    serialNumber: string;
    ownerName: string;
    ownerEmail: string;
    propertyAddress: string;
    connectionStatus: MeterConnectionStatus;
    lastSeenAtUtc: string | null;
    currentMonthKwh: number;
    lastBillStatus: "Placen" | "Neplacen" | "Nema racuna";
    approvedPowerKw: number;
};
