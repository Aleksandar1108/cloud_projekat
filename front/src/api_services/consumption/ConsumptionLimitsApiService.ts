import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import type { ConsumptionLimitSummary } from "../../types/consumption/ConsumptionLimitSummary";

function extractAxiosMessage(error: unknown, fallback: string): string {
    if (axios.isAxiosError(error) && error.response?.data) {
        const data = error.response.data as { message?: string; error?: string };
        return data.message ?? data.error ?? fallback;
    }
    return error instanceof Error ? error.message : fallback;
}

export async function getConsumptionLimitSummaries(userId: string): Promise<ConsumptionLimitSummary[]> {
    try {
        const res = await axios.get<ConsumptionLimitSummary[]>(buildApiUrl(`api/ConsumptionLimits/summary/${userId}`));
        return res.data;
    } catch (error) {
        throw new Error(extractAxiosMessage(error, "Neuspelo učitavanje pregleda limita."));
    }
}

export async function setConsumptionLimit(input: {
    userId: string;
    deviceId: string;
    limitKwh: number;
}): Promise<void> {
    try {
        await axios.put(buildApiUrl("api/ConsumptionLimits"), {
            userId: input.userId,
            deviceId: input.deviceId,
            limitKwh: input.limitKwh,
        });
    } catch (error) {
        throw new Error(extractAxiosMessage(error, "Čuvanje limita nije uspelo."));
    }
}
