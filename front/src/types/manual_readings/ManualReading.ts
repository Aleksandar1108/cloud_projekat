export type ManualReadingStatus = "Pending" | "Processed";

export type ManualReading = {
    id: string;
    deviceId: string;
    readingKwh: number;
    readingAtUtc: string;
    submitterEmail: string;
    status: ManualReadingStatus;
    rawImagePath: string;
    optimizedImagePath: string;
    createdAtUtc: string;
};
