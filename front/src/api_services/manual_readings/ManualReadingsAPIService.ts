import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import type { ManualReading } from "../../types/manual_readings/ManualReading";

function extractError(error: unknown, fallback: string): Error {
    if (axios.isAxiosError(error)) {
        const data = error.response?.data as { message?: string } | undefined;
        return new Error(data?.message ?? fallback);
    }
    return new Error(fallback);
}

export async function submitManualReading(input: {
    meterName: string;
    readingKwh: number;
    readingAtUtc: string;
    submitterEmail: string;
    meterImage: File;
}): Promise<ManualReading> {
    const form = new FormData();
    form.append("meterName", input.meterName);
    form.append("readingKwh", String(input.readingKwh));
    form.append("readingAtUtc", input.readingAtUtc);
    form.append("submitterEmail", input.submitterEmail);
    form.append("meterImage", input.meterImage);

    try {
        const res = await axios.post<ManualReading>(buildApiUrl("api/manual-readings"), form, {
            headers: { "Content-Type": "multipart/form-data" }
        });
        return res.data;
    } catch (error) {
        throw extractError(error, "Neuspesno slanje manuelnog ocitavanja.");
    }
}

export async function getManualReadings(status?: "Pending" | "Processed"): Promise<ManualReading[]> {
    const query = status ? `?status=${status}` : "";
    const res = await axios.get<ManualReading[]>(buildApiUrl(`api/manual-readings${query}`));
    return res.data;
}
