import { ReadValueByKey, SaveValueByKey } from "../../helpers/local_storage";
import {
    DEFAULT_TARIFF_MODEL,
    MOCK_BILLING_RUNS,
    MOCK_CONSUMPTION_SNAPSHOTS,
    MOCK_EMAIL_DELIVERY_LOGS,
    MOCK_METERS,
    MOCK_PAYMENTS,
} from "../../mock_data/admin/adminMockData";
import type { TariffModel } from "../../types/admin/TariffModel";
import type { MeterNetworkStatus } from "../../types/admin/MeterNetworkStatus";
import type { AdminPayment } from "../../types/admin/AdminPayment";
import type { AdminGeneratedBill, BillingRun } from "../../types/admin/BillingRun";
import type { BillingDeliveryStats, EmailDeliveryLog } from "../../types/admin/EmailDelivery";

const STORAGE_KEYS = {
    tariff: "admin_mock_tariff_model",
    billingRuns: "admin_mock_billing_runs",
    emailLogs: "admin_mock_email_logs",
} as const;

const delay = (ms = 400) => new Promise((resolve) => setTimeout(resolve, ms));

function readJson<T>(key: string, fallback: T): T {
    const raw = ReadValueByKey(key);
    if (!raw) return fallback;
    try {
        return JSON.parse(raw) as T;
    } catch {
        return fallback;
    }
}

function writeJson<T>(key: string, value: T): void {
    SaveValueByKey(key, JSON.stringify(value));
}

function calculateBill(
    snapshot: (typeof MOCK_CONSUMPTION_SNAPSHOTS)[number],
    tariff: TariffModel,
    year: number,
    month: number,
    index: number
): AdminGeneratedBill {
    const vtKwh = snapshot.endVtKwh - snapshot.startVtKwh;
    const ntKwh = snapshot.endNtKwh - snapshot.startNtKwh;
    const totalKwh = vtKwh + ntKwh;
    const kvt = totalKwh > 0 ? vtKwh / totalKwh : 0;
    const knt = totalKwh > 0 ? ntKwh / totalKwh : 0;

    const greenMax = tariff.zoneThresholds.greenMaxKwh;
    const blueMax = tariff.zoneThresholds.blueMaxKwh;

    const greenTotal = Math.min(totalKwh, greenMax);
    const blueTotal = Math.max(0, Math.min(totalKwh - greenMax, blueMax - greenMax));
    const redTotal = Math.max(0, totalKwh - blueMax);

    const zvt = greenTotal * kvt;
    const znt = greenTotal * knt;
    const pvt = blueTotal * kvt;
    const pnt = blueTotal * knt;
    const cvt = redTotal * kvt;
    const cnt = redTotal * knt;

    const iznosZelena =
        zvt * tariff.zonePrices.green.vtPriceRsd + znt * tariff.zonePrices.green.ntPriceRsd;
    const iznosPlava =
        pvt * tariff.zonePrices.blue.vtPriceRsd + pnt * tariff.zonePrices.blue.ntPriceRsd;
    const iznosCrvena =
        cvt * tariff.zonePrices.red.vtPriceRsd + cnt * tariff.zonePrices.red.ntPriceRsd;

    const fixedCosts =
        tariff.fixedCosts.billingPowerFeeRsd * snapshot.approvedPowerKw +
        tariff.fixedCosts.supplierCostRsd;
    const totalCost = iznosZelena + iznosPlava + iznosCrvena + fixedCosts;

    const invoiceId = `INV-${year}-${String(month).padStart(2, "0")}-${String(index + 1).padStart(4, "0")}`;
    const billText = [
        `RACUN ZA ELEKTRICNU ENERGIJU`,
        `Period: ${year}-${String(month).padStart(2, "0")}`,
        `Uredjaj: ${snapshot.deviceId}`,
        `Potrosac: ${snapshot.ownerName}`,
        ``,
        `Potrosnja VT: ${vtKwh.toFixed(2)} kWh`,
        `Potrosnja NT: ${ntKwh.toFixed(2)} kWh`,
        `Ukupno: ${totalKwh.toFixed(2)} kWh`,
        ``,
        `Zelena zona: ${greenTotal.toFixed(2)} kWh`,
        `Plava zona: ${blueTotal.toFixed(2)} kWh`,
        `Crvena zona: ${redTotal.toFixed(2)} kWh`,
        ``,
        `Energetski troskovi: ${(iznosZelena + iznosPlava + iznosCrvena).toFixed(2)} RSD`,
        `Fiksni troskovi: ${fixedCosts.toFixed(2)} RSD`,
        `UKUPNO ZA UPLATU: ${totalCost.toFixed(2)} RSD`,
    ].join("\n");

    return {
        invoiceId,
        deviceId: snapshot.deviceId,
        ownerName: snapshot.ownerName,
        ownerEmail: snapshot.ownerEmail,
        year,
        month,
        vtKwh: Number(vtKwh.toFixed(2)),
        ntKwh: Number(ntKwh.toFixed(2)),
        totalKwh: Number(totalKwh.toFixed(2)),
        greenZoneKwh: Number(greenTotal.toFixed(2)),
        blueZoneKwh: Number(blueTotal.toFixed(2)),
        redZoneKwh: Number(redTotal.toFixed(2)),
        energyCostRsd: Number((iznosZelena + iznosPlava + iznosCrvena).toFixed(2)),
        fixedCostsRsd: Number(fixedCosts.toFixed(2)),
        totalCostRsd: Number(totalCost.toFixed(2)),
        billText,
        emailSent: false,
    };
}

export async function getTariffModel(): Promise<TariffModel> {
    await delay();
    return readJson(STORAGE_KEYS.tariff, DEFAULT_TARIFF_MODEL);
}

export async function saveTariffModel(model: TariffModel): Promise<TariffModel> {
    await delay(600);
    const updated: TariffModel = {
        ...model,
        updatedAtUtc: new Date().toISOString(),
    };
    writeJson(STORAGE_KEYS.tariff, updated);
    return updated;
}

export async function getMeterStatuses(): Promise<MeterNetworkStatus[]> {
    await delay();
    return MOCK_METERS;
}

export async function getRealizedPayments(): Promise<AdminPayment[]> {
    await delay();
    return MOCK_PAYMENTS;
}

export async function getBillingRuns(): Promise<BillingRun[]> {
    await delay();
    return readJson(STORAGE_KEYS.billingRuns, MOCK_BILLING_RUNS);
}

export async function runMonthlyBilling(year: number, month: number): Promise<BillingRun> {
    await delay(1200);
    const tariff = await getTariffModel();
    const startedAt = new Date().toISOString();

    const bills = MOCK_CONSUMPTION_SNAPSHOTS.map((snapshot, index) =>
        calculateBill(snapshot, tariff, year, month, index)
    );

    const emailLogs: EmailDeliveryLog[] = bills.map((bill, index) => {
        const failed = index === 3;
        return {
            id: `email-${year}-${month}-${index + 1}`,
            invoiceId: bill.invoiceId,
            deviceId: bill.deviceId,
            ownerEmail: bill.ownerEmail,
            period: `${year}-${String(month).padStart(2, "0")}`,
            sentAtUtc: new Date().toISOString(),
            status: failed ? "Failed" : "Sent",
            errorMessage: failed ? "SMTP: Mailbox unavailable" : undefined,
        };
    });

    bills.forEach((bill, index) => {
        bill.emailSent = emailLogs[index].status === "Sent";
    });

    const run: BillingRun = {
        id: `run-${year}-${month}-${Date.now()}`,
        year,
        month,
        startedAtUtc: startedAt,
        completedAtUtc: new Date().toISOString(),
        status: "Completed",
        processedMeters: bills.length,
        generatedBills: bills.length,
        emailsSent: emailLogs.filter((l) => l.status === "Sent").length,
        bills,
    };

    const existingRuns = readJson<BillingRun[]>(STORAGE_KEYS.billingRuns, MOCK_BILLING_RUNS);
    writeJson(STORAGE_KEYS.billingRuns, [run, ...existingRuns]);

    const existingLogs = readJson<EmailDeliveryLog[]>(STORAGE_KEYS.emailLogs, MOCK_EMAIL_DELIVERY_LOGS);
    writeJson(STORAGE_KEYS.emailLogs, [...emailLogs, ...existingLogs]);

    return run;
}

export async function getEmailDeliveryLogs(): Promise<EmailDeliveryLog[]> {
    await delay();
    return readJson(STORAGE_KEYS.emailLogs, MOCK_EMAIL_DELIVERY_LOGS);
}

export async function getBillingDeliveryStats(): Promise<BillingDeliveryStats> {
    await delay();
    const logs = await getEmailDeliveryLogs();
    const runs = await getBillingRuns();
    const totalGenerated = runs.reduce((sum, run) => sum + run.generatedBills, 0);
    const totalSent = logs.filter((l) => l.status === "Sent").length;
    const totalFailed = logs.filter((l) => l.status === "Failed").length;
    const lastRun = runs[0]?.completedAtUtc ?? null;

    return {
        totalGenerated,
        totalSent,
        totalFailed,
        lastRunAtUtc: lastRun,
        deliveryRatePercent: totalGenerated > 0 ? Math.round((totalSent / logs.length) * 100) : 0,
    };
}

export async function resetAdminMockData(): Promise<void> {
    await delay(300);
    writeJson(STORAGE_KEYS.tariff, DEFAULT_TARIFF_MODEL);
    writeJson(STORAGE_KEYS.billingRuns, MOCK_BILLING_RUNS);
    writeJson(STORAGE_KEYS.emailLogs, MOCK_EMAIL_DELIVERY_LOGS);
}
