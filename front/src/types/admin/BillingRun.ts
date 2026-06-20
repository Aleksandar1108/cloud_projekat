export type AdminGeneratedBill = {
    invoiceId: string;
    deviceId: string;
    ownerName: string;
    ownerEmail: string;
    year: number;
    month: number;
    vtKwh: number;
    ntKwh: number;
    totalKwh: number;
    greenZoneKwh: number;
    blueZoneKwh: number;
    redZoneKwh: number;
    energyCostRsd: number;
    fixedCostsRsd: number;
    totalCostRsd: number;
    billText: string;
    emailSent: boolean;
};

export type BillingRun = {
    id: string;
    year: number;
    month: number;
    startedAtUtc: string;
    completedAtUtc: string;
    status: "Completed" | "Failed";
    processedMeters: number;
    generatedBills: number;
    emailsSent: number;
    bills: AdminGeneratedBill[];
};

export type MeterConsumptionSnapshot = {
    deviceId: string;
    ownerName: string;
    ownerEmail: string;
    startVtKwh: number;
    startNtKwh: number;
    endVtKwh: number;
    endNtKwh: number;
    approvedPowerKw: number;
};
