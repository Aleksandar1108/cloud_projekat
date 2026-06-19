import axios from "axios";
import type { MonthlyBill } from "../../types/billing/MonthlyBill";
import type { BillingStats } from "../../types/billing/BillingStats";
import { buildApiUrl } from "../ApiBase";

export async function getBillingStats(year: number, month: number): Promise<BillingStats> {
    try {
        const response = await axios.get<BillingStats>(
            buildApiUrl(`api/monthly-billing/stats?year=${year}&month=${month}`)
        );
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error) && error.response?.data) {
            const serverMessage = error.response.data.message ?? error.response.data.error;
            throw new Error(serverMessage ?? "Neuspesno ucitavanje statistike.");
        }
        throw new Error("Neuspesno ucitavanje statistike.");
    }
}

export async function getMonthlyBilling(year: number, month: number): Promise<MonthlyBill[]> {
    try {
        const response = await axios.get<MonthlyBill[]>(
            buildApiUrl(`api/monthly-billing?year=${year}&month=${month}`)
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error) && error.response?.data) {
            const serverMessage = error.response.data.message ?? error.response.data.error;
            throw new Error(serverMessage ?? "Neuspesno pokretanje mesecnog obracuna.");
        }

        throw new Error("Neuspesno pokretanje mesecnog obracuna.");
    }
}

export async function runMonthlyBilling(year: number, month: number): Promise<MonthlyBill[]> {
    try {
        const response = await axios.post<MonthlyBill[]>(
            buildApiUrl(`api/monthly-billing/run?year=${year}&month=${month}`),
            {}
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error) && error.response?.data) {
            const serverMessage = error.response.data.message ?? error.response.data.error;
            throw new Error(serverMessage ?? "Neuspesno pokretanje obracuna.");
        }

        throw new Error("Neuspesno pokretanje obracuna.");
    }
}
