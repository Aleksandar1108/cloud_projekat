export type ConsumptionLimitSummary = {
    deviceId: string;
    deviceName: string;
    limitKwh: number;
    monthConsumptionKwh: number;
    limitExceeded: boolean;
    notificationSentThisMonth: boolean;
};
