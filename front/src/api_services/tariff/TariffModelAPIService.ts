import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import type { TariffModel, TariffModelInput } from "../../models/tariff/TariffModel";

type ApiResult = { error?: string };

function extractError(error: unknown, fallback: string): string {
    if (axios.isAxiosError(error) && error.response) {
        const data = error.response.data;
        return data?.message || data?.error || data?.type || fallback;
    }
    return fallback;
}

export const TariffModelAPIService = {
    async getAll(): Promise<TariffModel[]> {
        try {
            const res = await axios.get<TariffModel[]>(buildApiUrl("api/tariff-models"));
            return res.data;
        } catch (error) {
            console.error(extractError(error, "Failed to load tariff models."));
            return [];
        }
    },

    async create(model: TariffModelInput): Promise<ApiResult> {
        try {
            await axios.post(buildApiUrl("api/tariff-models"), model);
            return {};
        } catch (error) {
            return { error: extractError(error, "Failed to create tariff model.") };
        }
    },

    async update(id: number, model: TariffModelInput): Promise<ApiResult> {
        try {
            await axios.put(buildApiUrl(`api/tariff-models/${id}`), model);
            return {};
        } catch (error) {
            return { error: extractError(error, "Failed to update tariff model.") };
        }
    },

    async activate(id: number): Promise<ApiResult> {
        try {
            await axios.patch(buildApiUrl(`api/tariff-models/${id}/activate`), {});
            return {};
        } catch (error) {
            return { error: extractError(error, "Failed to activate tariff model.") };
        }
    }
};
