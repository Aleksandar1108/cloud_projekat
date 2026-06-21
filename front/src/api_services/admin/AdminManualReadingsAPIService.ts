import axios from "axios";
import { ReadValueByKey } from "../../helpers/local_storage";
import { buildApiUrl } from "../ApiBase";
import type { ManualReading } from "../../types/manual_readings/ManualReading";

function authHeaders() {
    const token = ReadValueByKey("jwt");
    return token ? { Authorization: `Bearer ${token}` } : {};
}

function extractError(error: unknown, fallback: string): Error {
    if (axios.isAxiosError(error)) {
        if (error.response?.status === 401 || error.response?.status === 403) {
            return new Error("Nemate dozvolu za odobravanje ocitavanja.");
        }
        const data = error.response?.data as { message?: string } | undefined;
        return new Error(data?.message ?? fallback);
    }
    return new Error(fallback);
}

export async function getPendingManualReadings(): Promise<ManualReading[]> {
    try {
        const response = await axios.get<ManualReading[]>(
            buildApiUrl("api/admin/manual-readings/pending"),
            { headers: authHeaders() }
        );
        return response.data;
    } catch (error) {
        throw extractError(error, "Neuspesno ucitavanje pending ocitavanja.");
    }
}

export async function fetchManualReadingImageBlob(id: string, variant: "optimized" | "raw" = "optimized"): Promise<Blob> {
    const response = await axios.get<Blob>(
        buildApiUrl(`api/admin/manual-readings/${id}/image?variant=${variant}`),
        {
            headers: authHeaders(),
            responseType: "blob",
        }
    );
    return response.data;
}

export async function approveManualReadingAdmin(id: string): Promise<void> {
    try {
        await axios.post(
            buildApiUrl(`api/admin/manual-readings/${id}/approve`),
            {},
            { headers: authHeaders() }
        );
    } catch (error) {
        throw extractError(error, "Neuspesno odobravanje ocitavanja.");
    }
}

export async function rejectManualReadingAdmin(id: string): Promise<void> {
    try {
        await axios.post(
            buildApiUrl(`api/admin/manual-readings/${id}/reject`),
            {},
            { headers: authHeaders() }
        );
    } catch (error) {
        throw extractError(error, "Neuspesno odbijanje ocitavanja.");
    }
}
