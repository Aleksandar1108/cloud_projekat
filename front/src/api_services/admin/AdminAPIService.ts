import type { TariffModel } from "../../types/admin/TariffModel";
import type { MeterNetworkStatus } from "../../types/admin/MeterNetworkStatus";
import type { AdminPayment } from "../../types/admin/AdminPayment";
import type { BillingRun } from "../../types/admin/BillingRun";
import type { BillingDeliveryStats } from "../../types/admin/EmailDelivery";
import axios from "axios";
import { ReadValueByKey } from "../../helpers/local_storage";
import { buildApiUrl } from "../ApiBase";

function authHeaders() {
    const token = ReadValueByKey("jwt");
    return token ? { Authorization: `Bearer ${token}` } : {};
}

function extractError(error: unknown, fallback: string): Error {
    if (axios.isAxiosError(error)) {
        if (error.response?.status === 401) {
            return new Error("Niste prijavljeni kao admin. Uloguj se nalogom sa ulogom Admin.");
        }
        const data = error.response?.data as { message?: string } | undefined;
        return new Error(data?.message ?? fallback);
    }
    return new Error(fallback);
}

export async function getTariffModel(): Promise<TariffModel> {
    try {
        const response = await axios.get<TariffModel>(
            buildApiUrl("api/admin/tariffs"),
            { headers: authHeaders() }
        );
        return response.data;
    } catch (error) {
        throw extractError(error, "Neuspesno ucitavanje tarifnog modela.");
    }
}

export async function saveTariffModel(model: TariffModel): Promise<TariffModel> {
    try {
        const response = await axios.put<TariffModel>(
            buildApiUrl("api/admin/tariffs"),
            model,
            { headers: authHeaders() }
        );
        return response.data;
    } catch (error) {
        throw extractError(error, "Neuspesno cuvanje tarifnog modela.");
    }
}

export async function getMeterStatuses(): Promise<MeterNetworkStatus[]> {
    const response = await axios.get<MeterNetworkStatus[]>(
        buildApiUrl("api/admin/network/meters"),
        { headers: authHeaders() }
    );
    return response.data;
}

export async function getRealizedPayments(): Promise<AdminPayment[]> {
    const response = await axios.get<AdminPayment[]>(
        buildApiUrl("api/admin/payments"),
        { headers: authHeaders() }
    );
    return response.data;
}

export async function getBillingRuns(): Promise<BillingRun[]> {
    const response = await axios.get<BillingRun[]>(
        buildApiUrl("api/admin/billing/runs"),
        { headers: authHeaders() }
    );
    return response.data;
}

export async function runMonthlyBilling(year: number, month: number): Promise<BillingRun> {
    try {
        await axios.post(
            buildApiUrl(`api/admin/billing/run?year=${year}&month=${month}`),
            null,
            { headers: authHeaders() }
        );
    } catch (error) {
        throw extractError(error, "Neuspesno pokretanje mesecnog obracuna.");
    }

    const runs = await getBillingRuns();
    const existingRun = runs.find((run) => run.year === year && run.month === month);
    if (existingRun) {
        return existingRun;
    }

    const now = new Date().toISOString();
    return {
        id: `run-${year}-${month}-${Date.now()}`,
        year,
        month,
        startedAtUtc: now,
        completedAtUtc: now,
        status: "Completed",
        processedMeters: 0,
        generatedBills: 0,
        emailsSent: 0,
        bills: [],
    };
}

export async function getBillingDeliveryStats(): Promise<BillingDeliveryStats> {
    const response = await axios.get<BillingDeliveryStats>(
        buildApiUrl("api/admin/delivery/stats"),
        { headers: authHeaders() }
    );
    return response.data;
}

export async function getEmailDeliveryLogs() {
    const runs = await getBillingRuns();
    return runs.flatMap((run) =>
        run.bills.map((bill) => ({
            id: `${bill.invoiceId}-email`,
            invoiceId: bill.invoiceId,
            deviceId: bill.deviceId,
            ownerEmail: bill.ownerEmail,
            period: `${run.year}-${String(run.month).padStart(2, "0")}`,
            sentAtUtc: run.completedAtUtc,
            status: bill.emailSent ? ("Sent" as const) : ("Failed" as const),
            errorMessage: bill.emailSent ? undefined : "Slanje nije potvrdjeno",
        }))
    );
}
