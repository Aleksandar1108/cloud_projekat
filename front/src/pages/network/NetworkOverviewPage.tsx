import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getDeviceStatuses } from "../../api_services/devices/DeviceStatusAPIService";
import type { DeviceStatus } from "../../types/devices/DeviceStatus";

const REFRESH_INTERVAL_MS = 10000;

function StatusDot({ online }: { online: boolean }) {
    return (
        <span style={{
            display: "inline-block",
            width: "10px",
            height: "10px",
            borderRadius: "50%",
            backgroundColor: online ? "#16a34a" : "#dc2626",
            marginRight: "8px"
        }} />
    );
}

function NetworkOverviewPage() {
    const navigate = useNavigate();

    const [devices, setDevices] = useState<DeviceStatus[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [lastUpdated, setLastUpdated] = useState<Date | null>(null);
    const [autoRefresh, setAutoRefresh] = useState(true);

    const load = useCallback(async () => {
        try {
            const data = await getDeviceStatuses();
            setDevices(data);
            setLastUpdated(new Date());
            setError(null);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno ucitavanje statusa brojila.");
        }
    }, []);

    const timerRef = useRef<number | null>(null);

    useEffect(() => {
        void load();
        if (autoRefresh) {
            timerRef.current = window.setInterval(() => void load(), REFRESH_INTERVAL_MS);
        }
        return () => {
            if (timerRef.current !== null) {
                window.clearInterval(timerRef.current);
            }
        };
    }, [load, autoRefresh]);

    const onlineCount = devices.filter((d) => d.isOnline).length;
    const offlineCount = devices.length - onlineCount;

    return (
        <main style={{ width: "100%", maxWidth: "1100px", margin: "0 auto", padding: "24px" }}>
            <section style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "14px" }}>
                <div>
                    <h1 style={{ margin: 0, fontSize: "40px" }}>Pregled mreze</h1>
                    <p style={{ color: "#6b7280" }}>Status svih brojila u realnom vremenu (online/offline, opterecenje, firmware).</p>
                </div>
                <button
                    onClick={() => navigate("/")}
                    style={{ background: "#eef2ff", border: "1px solid #dbeafe", borderRadius: "10px", padding: "10px 14px", cursor: "pointer", fontWeight: 700 }}
                >
                    Nazad na dashboard
                </button>
            </section>

            <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: "10px", marginBottom: "12px" }}>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Ukupno brojila</p>
                    <h3 style={{ margin: "4px 0" }}>{devices.length}</h3>
                </article>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Online</p>
                    <h3 style={{ margin: "4px 0", color: "#16a34a" }}>{onlineCount}</h3>
                </article>
                <article style={{ border: "1px solid #e5e7eb", borderRadius: "10px", padding: "10px" }}>
                    <p style={{ fontSize: "13px", color: "#6b7280", margin: 0 }}>Offline</p>
                    <h3 style={{ margin: "4px 0", color: "#dc2626" }}>{offlineCount}</h3>
                </article>
            </div>

            <section style={{ display: "flex", gap: "10px", alignItems: "center", marginBottom: "12px" }}>
                <button onClick={() => void load()}
                    style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "8px 12px", cursor: "pointer", fontWeight: 700 }}>
                    Osvezi
                </button>
                <label style={{ display: "flex", alignItems: "center", gap: "6px", fontSize: "14px", color: "#374151" }}>
                    <input type="checkbox" checked={autoRefresh} onChange={(e) => setAutoRefresh(e.target.checked)} />
                    Auto-osvezavanje (10s)
                </label>
                {lastUpdated && <span style={{ color: "#6b7280", fontSize: "13px" }}>Azurirano: {lastUpdated.toLocaleTimeString()}</span>}
            </section>

            {error && <p style={{ color: "#dc2626" }}>{error}</p>}

            <section style={{ border: "1px solid #e5e7eb", borderRadius: "10px", overflow: "hidden" }}>
                <table style={{ width: "100%", borderCollapse: "collapse" }}>
                    <thead style={{ backgroundColor: "#f3f4f6" }}>
                        <tr>
                            <th style={{ textAlign: "left", padding: "10px" }}>Status</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Device</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Tip</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Snaga (kW)</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Opterecenje</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Firmware</th>
                        </tr>
                    </thead>
                    <tbody>
                        {devices.length === 0 && (
                            <tr><td colSpan={6} style={{ padding: "16px", color: "#6b7280" }}>Nema podataka o statusu brojila.</td></tr>
                        )}
                        {devices.map((d) => (
                            <tr key={d.deviceId} style={{ borderTop: "1px solid #f3f4f6" }}>
                                <td style={{ padding: "10px" }}>
                                    <StatusDot online={d.isOnline} />
                                    {d.isOnline ? "Online" : "Offline"}
                                </td>
                                <td style={{ padding: "10px" }}>
                                    {d.label ? (
                                        <>
                                            <div>{d.label}</div>
                                            <div style={{ fontSize: "12px", color: "#6b7280" }}>{d.deviceId}</div>
                                        </>
                                    ) : d.deviceId}
                                </td>
                                <td style={{ padding: "10px" }}>{d.deviceType}</td>
                                <td style={{ padding: "10px" }}>{d.currentPower.toFixed(2)}</td>
                                <td style={{ padding: "10px", color: d.isOverloaded ? "#dc2626" : d.isUnderperforming ? "#d97706" : "#111827" }}>
                                    {d.loadPercentage.toFixed(0)}%
                                    {d.isOverloaded ? " (preopterecen)" : d.isUnderperforming ? " (nedovoljno)" : ""}
                                </td>
                                <td style={{ padding: "10px" }}>
                                    {d.currentFirmwareVersion}
                                    {d.targetFirmwareVersion && d.targetFirmwareVersion !== d.currentFirmwareVersion
                                        ? ` -> ${d.targetFirmwareVersion} (${d.updateStatus})`
                                        : ""}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </section>
        </main>
    );
}

export default NetworkOverviewPage;
