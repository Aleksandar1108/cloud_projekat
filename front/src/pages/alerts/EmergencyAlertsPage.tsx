import { Navigate, useNavigate } from "react-router-dom";
import { useCallback, useEffect, useMemo, useState, type CSSProperties } from "react";
import LoginButton from "../../components/auth/LoginButton";
import { useAuth } from "../../hooks/auth/useAuthHook";
import {
    getConsumptionLimitSummaries,
} from "../../api_services/consumption/ConsumptionLimitsApiService";
import type { ConsumptionLimitSummary } from "../../types/consumption/ConsumptionLimitSummary";

const card: CSSProperties = {
    maxWidth: "960px",
    margin: "0 auto",
    padding: "28px 32px",
    background: "var(--white)",
    borderRadius: "12px",
    boxShadow: "0 8px 30px rgba(15, 34, 70, 0.08)",
    border: "1px solid var(--gray-200)",
};

const mono: CSSProperties = {
    fontFamily: "ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace",
    fontSize: "13px",
    background: "var(--gray-900)",
    color: "var(--gray-50)",
    padding: "2px 8px",
    borderRadius: "6px",
};

function EmergencyAlertsPage() {
    const navigate = useNavigate();
    const { isAuthenticated, user } = useAuth();
    const [rows, setRows] = useState<ConsumptionLimitSummary[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const periodLabel = useMemo(() => {
        const d = new Date();
        return `${d.getUTCFullYear()}-${String(d.getUTCMonth() + 1).padStart(2, "0")} (UTC)`;
    }, []);

    const load = useCallback(async () => {
        if (!user?.id) return;
        setLoading(true);
        setError(null);
        try {
            const data = await getConsumptionLimitSummaries(user.id);
            setRows(data);
        } catch (e) {
            setError(e instanceof Error ? e.message : "Neuspelo učitavanje.");
        } finally {
            setLoading(false);
        }
    }, [user?.id]);

    useEffect(() => {
        if (isAuthenticated && user?.id) void load();
    }, [isAuthenticated, user?.id, load]);

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    return (
        <main style={{ width: "100%", padding: "24px 16px 48px", boxSizing: "border-box" }}>
            <div style={{ maxWidth: "960px", margin: "0 auto 20px", display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: "12px" }}>
                <button
                    type="button"
                    onClick={() => navigate("/")}
                    style={{
                        background: "transparent",
                        border: "1px solid var(--gray-300)",
                        padding: "8px 14px",
                        borderRadius: "8px",
                        cursor: "pointer",
                        fontWeight: 600,
                    }}
                >
                    ← Nazad na kontrolnu tablu
                </button>
                <LoginButton />
            </div>

            <header style={{ textAlign: "center", marginBottom: "28px" }}>
                <h1 style={{ margin: "0 0 8px", fontSize: "1.75rem" }}>Hitna upozorenja i limit potrošnje</h1>
                <p style={{ margin: 0, color: "var(--gray-600)", maxWidth: "680px", marginInline: "auto", lineHeight: 1.55 }}>
                    Ovaj modul prikazuje podešene mesečne limite potrošnje po brojilu, agregat potrošnje za tekući UTC mesec
                    (iz telemetrije), status prekoračenja i evidenciju da li je obaveštenje o limitu već poslato u tom mesecu.
                </p>
            </header>

            <section style={card}>
                <h2 style={{ marginTop: 0, fontSize: "1.15rem" }}>Funkcionalna specifikacija — obaveštenja</h2>
                <p style={{ margin: "0 0 16px", color: "var(--gray-700)", lineHeight: 1.65 }}>
                    Nakon obrade ulazne telemetrije, sistem primenjuje pravila ispod. Vremenska osnova za mesečnu potrošnju
                    jeste kalendarski mesec u UTC zoni.
                </p>
                <p style={{ margin: "0 0 20px", color: "var(--gray-700)", lineHeight: 1.65 }}>
                    <strong>Nadzor napona (kritično upozorenje).</strong> Ako je u poslednjem merenju prijavljen napon
                    ispod konfigurisanog praga (podrazumevano 190&nbsp;V), generiše se upozorenje tipa{" "}
                    <em>kritično</em> namenjeno administratoru mreže.
                </p>
            </section>

            <section style={{ ...card, marginTop: "20px" }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "baseline", flexWrap: "wrap", gap: "12px", marginBottom: "16px" }}>
                    <h2 style={{ margin: 0, fontSize: "1.15rem" }}>Brojila i potrošnja (pregled stanja)</h2>
                    <span style={{ fontSize: "14px", color: "var(--gray-600)" }}>Mesec: {periodLabel}</span>
                </div>
                <button
                    type="button"
                    onClick={() => void load()}
                    disabled={loading}
                    style={{
                        marginBottom: "16px",
                        backgroundColor: "var(--secondary)",
                        color: "var(--white)",
                        border: "none",
                        padding: "8px 16px",
                        borderRadius: "8px",
                        cursor: loading ? "wait" : "pointer",
                        fontWeight: 600,
                    }}
                >
                    Osveži
                </button>
                {error && (
                    <p role="alert" style={{ color: "var(--red-700)", marginTop: 0 }}>
                        {error}
                    </p>
                )}
                {loading && <p style={{ color: "var(--gray-600)" }}>Učitavanje…</p>}
                {!loading && rows.length === 0 && (
                    <p style={{ color: "var(--gray-600)", margin: 0, lineHeight: 1.55 }}>
                        Za prijavljenog korisnika nije registrovan nijedan mesečni limit. Pregled će se pojaviti kada limit
                        bude dodeljen putem servisa ili drugog administratorskog kanala. 
                    </p>
                )}
                {!loading && rows.length > 0 && (
                    <div style={{ overflowX: "auto" }}>
                        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: "14px" }}>
                            <thead>
                                <tr style={{ textAlign: "left", borderBottom: "2px solid var(--gray-200)" }}>
                                    <th style={{ padding: "10px 8px" }}>Uređaj</th>
                                    <th style={{ padding: "10px 8px" }}>Limit (kWh)</th>
                                    <th style={{ padding: "10px 8px" }}>Potrošnja (kWh)</th>
                                    <th style={{ padding: "10px 8px" }}>Status</th>
                                    <th style={{ padding: "10px 8px" }}>E-mail poslat ovog meseca</th>
                                </tr>
                            </thead>
                            <tbody>
                                {rows.map((r) => (
                                    <tr
                                        key={r.deviceId}
                                        style={{
                                            borderBottom: "1px solid var(--gray-100)",
                                            background: r.limitExceeded ? "var(--red-50)" : "var(--blue-50)",
                                        }}
                                    >
                                        <td style={{ padding: "12px 8px", verticalAlign: "top" }}>
                                            <div style={{ fontWeight: 600 }}>{r.deviceName}</div>
                                            <div style={{ ...mono, marginTop: "6px", display: "inline-block", background: "var(--gray-800)", fontSize: "11px" }}>
                                                {r.deviceId}
                                            </div>
                                        </td>
                                        <td style={{ padding: "12px 8px" }}>{r.limitKwh}</td>
                                        <td style={{ padding: "12px 8px" }}>{r.monthConsumptionKwh.toFixed(2)}</td>
                                        <td style={{ padding: "12px 8px", fontWeight: 700, color: r.limitExceeded ? "var(--red-700)" : "var(--blue-700)" }}>
                                            {r.limitExceeded ? "Prekoračeno" : "U okviru limita"}
                                        </td>
                                        <td style={{ padding: "12px 8px" }}>{r.notificationSentThisMonth ? "Da" : "Ne"}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>
        </main>
    );
}

export default EmergencyAlertsPage;
