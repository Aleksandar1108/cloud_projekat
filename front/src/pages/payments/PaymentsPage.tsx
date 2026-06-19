import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getPayments } from "../../api_services/payments/PaymentsAPIService";
import type { Payment, PaymentStatus } from "../../types/payments/Payment";

const statusColor: Record<PaymentStatus, string> = {
    Paid: "#16a34a",
    Pending: "#d97706",
    Failed: "#dc2626",
    Canceled: "#6b7280"
};

function PaymentsPage() {
    const navigate = useNavigate();

    const now = new Date();
    const [year, setYear] = useState<number>(now.getUTCFullYear());
    const [month, setMonth] = useState<number>(now.getUTCMonth() + 1);
    const [payments, setPayments] = useState<Payment[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const load = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await getPayments(year, month);
            setPayments(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno ucitavanje uplata.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        void load();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const paid = payments.filter((p) => p.status === "Paid");
    const totalPaid = paid.reduce((sum, p) => sum + p.amountMinor, 0) / 100;

    return (
        <main style={{ width: "100%", maxWidth: "1000px", margin: "0 auto", padding: "24px" }}>
            <section style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "14px" }}>
                <div>
                    <h1 style={{ margin: 0, fontSize: "40px" }}>Uplate</h1>
                    <p style={{ color: "#6b7280" }}>Pregled realizovanih i pokrenutih uplata po periodu.</p>
                </div>
                <button
                    onClick={() => navigate("/")}
                    style={{ background: "#eef2ff", border: "1px solid #dbeafe", borderRadius: "10px", padding: "10px 14px", cursor: "pointer", fontWeight: 700 }}
                >
                    Nazad na dashboard
                </button>
            </section>

            <section style={{ display: "flex", gap: "8px", marginBottom: "12px", alignItems: "flex-end", flexWrap: "wrap" }}>
                <label style={{ display: "flex", flexDirection: "column", gap: "4px", fontSize: "13px", color: "#374151" }}>
                    Godina
                    <input type="number" value={year} onChange={(e) => setYear(Number(e.target.value))}
                        style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "8px 10px", width: "110px" }} />
                </label>
                <label style={{ display: "flex", flexDirection: "column", gap: "4px", fontSize: "13px", color: "#374151" }}>
                    Mesec
                    <input type="number" min={1} max={12} value={month} onChange={(e) => setMonth(Number(e.target.value))}
                        style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "8px 10px", width: "90px" }} />
                </label>
                <button onClick={load} disabled={loading}
                    style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 14px", cursor: "pointer", fontWeight: 700 }}>
                    {loading ? "Ucitavanje..." : "Ucitaj"}
                </button>
            </section>

            <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: "10px", marginBottom: "12px" }}>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Ukupno uplata</p>
                    <h3 style={{ margin: "4px 0" }}>{payments.length}</h3>
                </article>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Placeno</p>
                    <h3 style={{ margin: "4px 0" }}>{paid.length}</h3>
                </article>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Realizovan iznos</p>
                    <h3 style={{ margin: "4px 0" }}>{totalPaid.toFixed(2)} RSD</h3>
                </article>
            </div>

            {error && <p style={{ color: "#dc2626" }}>{error}</p>}

            <section style={{ border: "1px solid #e5e7eb", borderRadius: "10px", overflow: "hidden" }}>
                <table style={{ width: "100%", borderCollapse: "collapse" }}>
                    <thead style={{ backgroundColor: "#f3f4f6" }}>
                        <tr>
                            <th style={{ textAlign: "left", padding: "10px" }}>Device</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Period</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Iznos</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Status</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Placeno (UTC)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {!loading && payments.length === 0 && (
                            <tr><td colSpan={5} style={{ padding: "16px", color: "#6b7280" }}>Nema uplata za izabrani period.</td></tr>
                        )}
                        {payments.map((p) => (
                            <tr key={`${p.deviceId}-${p.createdAtUtc}`} style={{ borderTop: "1px solid #f3f4f6" }}>
                                <td style={{ padding: "10px" }}>{p.deviceId}</td>
                                <td style={{ padding: "10px" }}>{p.year}-{String(p.month).padStart(2, "0")}</td>
                                <td style={{ padding: "10px" }}>{(p.amountMinor / 100).toFixed(2)} {p.currency.toUpperCase()}</td>
                                <td style={{ padding: "10px", color: statusColor[p.status], fontWeight: 700 }}>{p.status}</td>
                                <td style={{ padding: "10px" }}>{p.paidAtUtc ? new Date(p.paidAtUtc).toLocaleString() : "-"}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </section>
        </main>
    );
}

export default PaymentsPage;
