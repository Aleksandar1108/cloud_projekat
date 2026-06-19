export type PaymentStatus = "Pending" | "Paid" | "Failed" | "Canceled";

export interface Payment {
    deviceId: string;
    year: number;
    month: number;
    amountMinor: number;
    currency: string;
    status: PaymentStatus;
    stripeSessionId: string | null;
    stripePaymentIntentId: string | null;
    createdAtUtc: string;
    paidAtUtc: string | null;
}
