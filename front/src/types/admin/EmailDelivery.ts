export type EmailDeliveryLog = {
    id: string;
    invoiceId: string;
    deviceId: string;
    ownerEmail: string;
    period: string;
    sentAtUtc: string;
    status: "Sent" | "Failed";
    errorMessage?: string;
};

export type BillingDeliveryStats = {
    totalGenerated: number;
    totalSent: number;
    totalFailed: number;
    lastRunAtUtc: string | null;
    deliveryRatePercent: number;
};
