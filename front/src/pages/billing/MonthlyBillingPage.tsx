import { Navigate, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { getMonthlyBilling, runMonthlyBilling, getBillingStats } from "../../api_services/billing/BillingAPIService";
import { createCheckoutSession } from "../../api_services/payments/PaymentsAPIService";
import type { MonthlyBill } from "../../types/billing/MonthlyBill";
import type { BillingStats } from "../../types/billing/BillingStats";
import { ERoles } from "../../enums/user/UserRole";

function MonthlyBillingPage() {
    const navigate = useNavigate();
    const { isAuthenticated, user } = useAuth();
    const [isLoading, setIsLoading] = useState(false);
    const [isPaying, setIsPaying] = useState(false);
    const [isRunning, setIsRunning] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [message, setMessage] = useState<string | null>(null);
    const [filter, setFilter] = useState("");
    const [draftFilter, setDraftFilter] = useState("");
    const [bills, setBills] = useState<MonthlyBill[]>([]);
    const [selectedIndex, setSelectedIndex] = useState<number | null>(null);

    const now = new Date();
    const [year, setYear] = useState<number>(now.getUTCFullYear());
    const [month, setMonth] = useState<number>(now.getUTCMonth() + 1);

    const [stats, setStats] = useState<BillingStats | null>(null);

    const isBillingAdmin = user?.role === ERoles.Admin || user?.role === ERoles.SysAdmin;

    if (!isAuthenticated) {
        return <Navigate to="/login" />;
    }

    const loadStats = async () => {
        if (!isBillingAdmin) return;
        try {
            const data = await getBillingStats(year, month);
            setStats(data);
        } catch {
            setStats(null);
        }
    };

    const handleLoadBilling = async () => {
        setIsLoading(true);
        setError(null);

        try {
            const response = await getMonthlyBilling(year, month);
            setBills(response);
            setSelectedIndex(response.length > 0 ? 0 : null);
            await loadStats();
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno pokretanje mesecnog obracuna.");
        } finally {
            setIsLoading(false);
        }
    };

    const handleRunBilling = async () => {
        setIsRunning(true);
        setError(null);
        setMessage(null);

        try {
            const response = await runMonthlyBilling(year, month);
            setBills(response);
            setSelectedIndex(response.length > 0 ? 0 : null);
            setMessage(
                response.length > 0
                    ? `Obracun zavrsen. Generisano racuna: ${response.length}.`
                    : "Obracun zavrsen, ali nema potrosnje za izabrani period."
            );
            await loadStats();
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno pokretanje obracuna.");
        } finally {
            setIsRunning(false);
        }
    };

    const filteredBills = bills.filter((bill) =>
        bill.deviceId.toLowerCase().includes(filter.toLowerCase())
    );

    const totalEnergy = filteredBills.reduce((sum, bill) => sum + bill.totalKwh, 0);
    const totalAmount = filteredBills.reduce((sum, bill) => sum + bill.totalCost, 0);
    const selectedBill = selectedIndex !== null ? filteredBills[selectedIndex] : null;

    const handlePaySelected = async () => {
        if (!selectedBill) return;

        setIsPaying(true);
        setError(null);

        try {
            const res = await createCheckoutSession(selectedBill.deviceId, selectedBill.year, selectedBill.month);
            window.location.assign(res.url);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno pokretanje placanja.");
        } finally {
            setIsPaying(false);
        }
    };

    useEffect(() => {
        void handleLoadBilling();
        void loadStats();
        // Load monthly billing view immediately; generation is done by backend schedule.
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return (
        <main style={{ width: "100%", maxWidth: "920px", margin: "0 auto", padding: "24px" }}>
            <section style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "14px" }}>
                <div>
                    <h1 style={{ margin: 0, fontSize: "42px" }}>Mesecni obracun</h1>
                    <p>Pregled i detalji svih generisanih racuna.</p>
                </div>
                <div style={{ display: "flex", gap: "10px" }}>
                    <button
                        onClick={handlePaySelected}
                        disabled={!selectedBill || isPaying}
                        style={{
                            backgroundColor: selectedBill ? "#16a34a" : "#d1d5db",
                            color: "white",
                            border: "none",
                            borderRadius: "10px",
                            padding: "10px 14px",
                            cursor: selectedBill && !isPaying ? "pointer" : "not-allowed",
                            fontWeight: 800
                        }}
                    >
                        {isPaying ? "Preusmeravanje..." : "Plati izabrani racun"}
                    </button>
                    <button
                        onClick={() => navigate("/")}
                        style={{ background: "#eef2ff", border: "1px solid #dbeafe", borderRadius: "10px", padding: "10px 14px", cursor: "pointer", fontWeight: 700 }}
                    >
                        Nazad na dashboard
                    </button>
                </div>
            </section>

            {isBillingAdmin && stats && (
                <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: "10px", marginBottom: "12px" }}>
                    <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px", textAlign: "left", background: "#f8fafc" }}>
                        <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Generisano racuna</p>
                        <h3 style={{ margin: "4px 0" }}>{stats.billCount}</h3>
                    </article>
                    <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px", textAlign: "left", background: "#f8fafc" }}>
                        <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Ukupan iznos</p>
                        <h3 style={{ margin: "4px 0" }}>{stats.totalAmount.toFixed(2)} RSD</h3>
                    </article>
                    <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px", textAlign: "left", background: "#f0fdf4" }}>
                        <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Placeno</p>
                        <h3 style={{ margin: "4px 0", color: "#16a34a" }}>{stats.paidCount} ({stats.paidAmount.toFixed(2)} RSD)</h3>
                    </article>
                    <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px", textAlign: "left", background: "#fef2f2" }}>
                        <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Neplaceno</p>
                        <h3 style={{ margin: "4px 0", color: "#dc2626" }}>{stats.unpaidCount} ({stats.unpaidAmount.toFixed(2)} RSD)</h3>
                    </article>
                </div>
            )}

            <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: "10px", marginBottom: "12px" }}>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px", textAlign: "left" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280" }}>Broj racuna</p>
                    <h3 style={{ margin: "4px 0" }}>{filteredBills.length}</h3>
                </article>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px", textAlign: "left" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280" }}>Ukupna energija</p>
                    <h3 style={{ margin: "4px 0" }}>{filteredBills.length > 0 ? `${totalEnergy.toFixed(2)} kWh` : "0 kWh"}</h3>
                </article>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px", textAlign: "left" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280" }}>Ukupan iznos</p>
                    <h3 style={{ margin: "4px 0" }}>{filteredBills.length > 0 ? `${totalAmount.toFixed(2)} RSD` : "0 RSD"}</h3>
                </article>
            </div>

            <section style={{ display: "flex", gap: "8px", marginBottom: "12px", alignItems: "flex-end", flexWrap: "wrap" }}>
                <label style={{ display: "flex", flexDirection: "column", gap: "4px", fontSize: "13px", color: "#374151" }}>
                    Godina
                    <input
                        type="number"
                        value={year}
                        onChange={(e) => setYear(Number(e.target.value))}
                        style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "8px 10px", width: "110px" }}
                    />
                </label>
                <label style={{ display: "flex", flexDirection: "column", gap: "4px", fontSize: "13px", color: "#374151" }}>
                    Mesec
                    <input
                        type="number"
                        min={1}
                        max={12}
                        value={month}
                        onChange={(e) => setMonth(Number(e.target.value))}
                        style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "8px 10px", width: "90px" }}
                    />
                </label>
                <button
                    onClick={handleLoadBilling}
                    disabled={isLoading}
                    style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 14px", cursor: "pointer", fontWeight: 700 }}
                >
                    {isLoading ? "Ucitavanje..." : "Ucitaj"}
                </button>
                {isBillingAdmin && (
                    <button
                        onClick={handleRunBilling}
                        disabled={isRunning}
                        style={{ backgroundColor: "#7c3aed", color: "white", border: "none", borderRadius: "8px", padding: "10px 14px", cursor: isRunning ? "not-allowed" : "pointer", fontWeight: 800 }}
                    >
                        {isRunning ? "Obracun u toku..." : "Pokreni obracun"}
                    </button>
                )}
            </section>

            {message && <p style={{ color: "#16a34a", marginBottom: "10px" }}>{message}</p>}

            <section style={{ display: "flex", gap: "8px", marginBottom: "12px" }}>
                <input
                    value={draftFilter}
                    onChange={(e) => setDraftFilter(e.target.value)}
                    placeholder="Filter po DeviceId"
                    style={{ flex: 1, border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 12px" }}
                />
                <button
                    onClick={() => setFilter(draftFilter)}
                    style={{ backgroundColor: "var(--secondary)", color: "var(--white)", border: "none", borderRadius: "8px", padding: "10px 12px", fontWeight: 700, cursor: "pointer" }}
                >
                    Primeni filter
                </button>
                <button
                    onClick={() => { setDraftFilter(""); setFilter(""); }}
                    style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 12px", cursor: "pointer" }}
                >
                    Reset
                </button>
            </section>

            {isLoading && <p style={{ marginBottom: "10px" }}>Ucitavanje...</p>}
            {error && <p style={{ color: "#dc2626", marginBottom: "10px" }}>{error}</p>}

            <section style={{ border: "1px solid #e5e7eb", borderRadius: "10px", overflow: "hidden", marginBottom: "14px" }}>
                <table style={{ width: "100%", borderCollapse: "collapse" }}>
                    <thead style={{ backgroundColor: "#f3f4f6" }}>
                        <tr>
                            <th style={{ textAlign: "left", padding: "10px" }}>InvoiceId</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Device</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Period</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>VT / NT (kWh)</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Ukupno (kWh)</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Iznos (RSD)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredBills.length === 0 && (
                            <tr>
                                <td colSpan={6} style={{ padding: "16px", color: "#6b7280" }}>Nema dostupnih racuna.</td>
                            </tr>
                        )}
                        {filteredBills.map((bill, index) => (
                            <tr
                                key={`${bill.deviceId}-${index}`}
                                onClick={() => setSelectedIndex(index)}
                                style={{ cursor: "pointer", backgroundColor: selectedIndex === index ? "#eff6ff" : "transparent", borderTop: "1px solid #f3f4f6" }}
                            >
                                <td style={{ padding: "10px" }}>INV-{String(index + 1).padStart(4, "0")}</td>
                                <td style={{ padding: "10px" }}>{bill.deviceId}</td>
                                <td style={{ padding: "10px" }}>{bill.year}-{String(bill.month).padStart(2, "0")}</td>
                                <td style={{ padding: "10px" }}>{bill.higherTariffKwh} / {bill.lowerTariffKwh}</td>
                                <td style={{ padding: "10px" }}>{bill.totalKwh}</td>
                                <td style={{ padding: "10px" }}>{bill.totalCost}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </section>

            <section style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "14px", textAlign: "left" }}>
                <h3 style={{ marginTop: 0 }}>Tekstualni racun</h3>
                <pre style={{ whiteSpace: "pre-wrap", margin: 0, color: "#374151" }}>
                    {selectedBill ? selectedBill.billText : "Izaberi racun iz tabele."}
                </pre>
            </section>
        </main>
    );
}

export default MonthlyBillingPage;
