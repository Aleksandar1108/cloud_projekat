import axios from "axios";
import type { MonthlyBill } from "../../types/billing/MonthlyBill";
import { buildApiUrl } from "../ApiBase";

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
