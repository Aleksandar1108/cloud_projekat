export type AdminPayment = {
    id: string;
    deviceId: string;
    ownerName: string;
    ownerEmail: string;
    invoiceId: string;
    period: string;
    amountRsd: number;
    paidAtUtc: string;
    paymentMethod: string;
    status: "Realizovana";
};
