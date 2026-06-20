import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import type { ManualReading } from "../../types/manual_readings/ManualReading";

export async function submitManualReading(input: {
    deviceId: string;
    readingKwh: number;
    readingAtUtc: string;
    submitterEmail: string;
    meterImage: File;
}): Promise<ManualReading> {
    const form = new FormData();
    form.append("deviceId", input.deviceId);
    form.append("readingKwh", String(input.readingKwh));
    form.append("readingAtUtc", input.readingAtUtc);
    form.append("submitterEmail", input.submitterEmail);
    form.append("meterImage", input.meterImage);

    const res = await axios.post<ManualReading>(buildApiUrl("api/manual-readings"), form, {
        headers: { "Content-Type": "multipart/form-data" }
    });
    return res.data;
}

export async function getManualReadings(status?: "Pending" | "Processed"): Promise<ManualReading[]> {
    const query = status ? `?status=${status}` : "";
    const res = await axios.get<ManualReading[]>(buildApiUrl(`api/manual-readings${query}`));
    return res.data;
}

export async function approveManualReading(id: string): Promise<void> {
    await axios.post(buildApiUrl(`api/manual-readings/${id}/approve`));
}
