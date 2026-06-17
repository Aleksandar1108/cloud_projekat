import { useEffect, useMemo, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { getProperties } from "../../api_services/properties/PropertyAPIService";
import { getPropertyTelemetryAnalytics } from "../../api_services/telemetry/TelemetryAnalyticsAPIService";
import { buildApiUrl } from "../../api_services/ApiBase";
import type { Property } from "../../types/property/Property";
import type { DeviceStatusSignalRDto, PropertyTelemetryAnalytics, SmartMeterTelemetryAnalytics } from "../../types/telemetry/TelemetryAnalytics";

function TelemetryAnalyticsPage() {
    const navigate = useNavigate();
    const [properties, setProperties] = useState<Property[]>([]);
    const [activePropertyId, setActivePropertyId] = useState<string | null>(null);
    const [analytics, setAnalytics] = useState<PropertyTelemetryAnalytics | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const connectionRef = useRef<HubConnection | null>(null);
    const lastGroupRef = useRef<string | null>(null);

    const activePropertyName = useMemo(
        () => properties.find(p => p.id === activePropertyId)?.name ?? "Nepoznati objekat",
        [properties, activePropertyId]
    );

    useEffect(() => {
        void initializePage();
        return () => {
            void stopConnection();
        };
    }, []);

    useEffect(() => {
        if (!activePropertyId) return;
        void loadAnalytics(activePropertyId);
        void switchPropertyGroup(activePropertyId);
    }, [activePropertyId]);

    async function initializePage() {
        try {
            setLoading(true);
            const props = await getProperties();
            setProperties(props);
            if (props.length > 0) {
                setActivePropertyId(props[0].id);
            }
            await ensureConnection();
        } catch {
            setError("Greška pri učitavanju telemetrije.");
        } finally {
            setLoading(false);
        }
    }

    async function loadAnalytics(propertyId: string) {
        try {
            setError(null);
            const data = await getPropertyTelemetryAnalytics(propertyId);
            setAnalytics(data);
        } catch {
            setError("Neuspelo učitavanje analytics podataka.");
        }
    }

    async function ensureConnection() {
        if (connectionRef.current && connectionRef.current.state !== HubConnectionState.Disconnected) {
            return;
        }

        const connection = new HubConnectionBuilder()
            .withUrl(buildApiUrl("device-status-hub"))
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Warning)
            .build();

        connection.on("ReceivePropertyStatusUpdate", (payload: DeviceStatusSignalRDto) => {
            setAnalytics(prev => {
                if (!prev) return prev;
                const updatedMeters = prev.meters.map(meter => applyRealtimeStatus(meter, payload));
                return { ...prev, generatedAtUtc: new Date().toISOString(), meters: updatedMeters };
            });
        });

        await connection.start();
        connectionRef.current = connection;
    }

    async function switchPropertyGroup(nextPropertyId: string) {
        const connection = connectionRef.current;
        if (!connection || connection.state !== HubConnectionState.Connected) return;

        const previous = lastGroupRef.current;
        if (previous && previous !== nextPropertyId) {
            await connection.invoke("LeavePropertyGroup", previous);
        }

        if (previous !== nextPropertyId) {
            await connection.invoke("JoinPropertyGroup", nextPropertyId);
            lastGroupRef.current = nextPropertyId;
        }
    }

    async function stopConnection() {
        const connection = connectionRef.current;
        if (!connection) return;

        try {
            if (connection.state === HubConnectionState.Connected && lastGroupRef.current) {
                await connection.invoke("LeavePropertyGroup", lastGroupRef.current);
            }
            await connection.stop();
        } finally {
            connectionRef.current = null;
            lastGroupRef.current = null;
        }
    }

    const meters = analytics?.meters ?? [];

    if (loading) {
        return <main style={pageStyle}><p>Učitavanje telemetry analytics...</p></main>;
    }

    return (
        <main style={pageStyle}>
            <div style={headerStyle}>
                <h1 style={{ margin: 0 }}>Telemetrija i analitika</h1>
                <button onClick={() => navigate("/properties")} style={btnSecondary}>← Nazad na objekte</button>
            </div>

            {error && <p style={{ color: "#dc2626", marginTop: "12px" }}>{error}</p>}

            <div style={tabsRowStyle}>
                {properties.map(p => (
                    <button
                        key={p.id}
                        onClick={() => setActivePropertyId(p.id)}
                        style={p.id === activePropertyId ? activeTabStyle : tabStyle}
                    >
                        {p.name}
                    </button>
                ))}
            </div>

            {properties.length === 0 ? (
                <section style={{ ...cardStyle, marginTop: "20px" }}>
                    <h2 style={{ marginTop: 0 }}>Telemetrija i analitika</h2>
                    <p style={{ marginBottom: "12px" }}>Nemate nijedan objekat za prikaz telemetrije.</p>
                    <button onClick={() => navigate("/properties")} style={btnSecondary}>
                        Idi na objekte
                    </button>
                </section>
            ) : (
                <h2 style={{ marginTop: "20px" }}>{activePropertyName}</h2>
            )}

            {properties.length > 0 && meters.length === 0 ? (
                <p>Za ovaj objekat trenutno nema uparenih brojila ili telemetrije.</p>
            ) : properties.length > 0 ? (
                <div style={{ display: "grid", gap: "16px" }}>
                    {meters.map(meter => (
                        <section key={meter.smartMeterId} style={cardStyle}>
                            <h3 style={{ marginTop: 0 }}>{meter.label}</h3>

                            <div style={statusGridStyle}>
                                <StatusCard label="Trenutna tarifa" value={meter.status.currentTariff} />
                                <StatusCard label="Trenutni napon" value={formatVoltage(meter.status.currentVoltage)} />
                                <StatusCard label="Status konekcije" value={meter.status.isOnline ? "Online" : "Offline"} />
                                <StatusCard label="Opterećenje" value={`${meter.status.loadPercentage.toFixed(2)} %`} />
                            </div>

                            <div style={chartsGridStyle}>
                                <div style={chartCardStyle}>
                                    <h4 style={chartTitleStyle}>Dnevna potrošnja (VT / NT)</h4>
                                    <DailyConsumptionChart meter={meter} />
                                </div>

                                <div style={chartCardStyle}>
                                    <h4 style={chartTitleStyle}>Trend napona i opterećenja (24h)</h4>
                                    <TrendChart meter={meter} />
                                    <small style={{ color: "#666" }}>
                                        Napon će biti prikazan kada telemetrijski payload pošalje voltage vrednosti.
                                    </small>
                                </div>
                            </div>
                        </section>
                    ))}
                </div>
            ) : null}
        </main>
    );
}

function applyRealtimeStatus(meter: SmartMeterTelemetryAnalytics, payload: DeviceStatusSignalRDto): SmartMeterTelemetryAnalytics {
    if (!meter.deviceUuid || meter.deviceUuid !== payload.deviceId) {
        return meter;
    }

    const voltage = resolvePayloadVoltage(payload) ?? meter.status.currentVoltage ?? null;

    return {
        ...meter,
        status: {
            ...meter.status,
            currentPowerKw: payload.currentPower,
            loadPercentage: payload.loadPercentage,
            isOnline: payload.isOnline,
            currentVoltage: voltage,
            currentTariff: resolveTariff(new Date()),
            lastHeartbeatUtc: new Date().toISOString()
        }
    };
}

function resolveTariff(date: Date): string {
    const hour = date.getHours();
    return hour >= 7 && hour < 23 ? "VT" : "NT";
}

function formatVoltage(value?: number | null): string {
    if (value === null || value === undefined) return "N/A";
    return `${value.toFixed(1)} V`;
}

function resolvePayloadVoltage(payload: DeviceStatusSignalRDto): number | null {
    const candidates = [payload.voltage, payload.currentVoltage];
    for (const candidate of candidates) {
        if (typeof candidate === "number" && Number.isFinite(candidate)) {
            return candidate;
        }
    }
    return null;
}

function StatusCard({ label, value }: { label: string; value: string }) {
    return (
        <div style={statusCardStyle}>
            <span style={{ fontSize: "12px", color: "#6b7280" }}>{label}</span>
            <strong style={{ fontSize: "16px", marginTop: "6px" }}>{value}</strong>
        </div>
    );
}

function DailyConsumptionChart({ meter }: { meter: SmartMeterTelemetryAnalytics }) {
    const points = meter.dailyConsumption.map(item => ({
        day: new Date(item.dayUtc).toLocaleDateString("sr-RS", { day: "2-digit", month: "2-digit" }),
        vt: item.higherTariffKwh,
        nt: item.lowerTariffKwh
    }));
    const maxValue = Math.max(1, ...points.map(p => Math.max(p.vt, p.nt)));

    return (
        <div style={{ display: "grid", gap: "8px", minHeight: "240px" }}>
            {points.map(point => (
                <div key={point.day} style={{ display: "grid", gridTemplateColumns: "56px 1fr", gap: "10px", alignItems: "center" }}>
                    <span style={{ fontSize: "12px", color: "#555" }}>{point.day}</span>
                    <div style={{ display: "grid", gap: "6px" }}>
                        <BarRow label="VT" value={point.vt} maxValue={maxValue} color="#2563eb" />
                        <BarRow label="NT" value={point.nt} maxValue={maxValue} color="#22c55e" />
                    </div>
                </div>
            ))}
        </div>
    );
}

function BarRow({ label, value, maxValue, color }: { label: string; value: number; maxValue: number; color: string }) {
    const width = `${Math.max(2, (value / maxValue) * 100)}%`;
    return (
        <div style={{ display: "grid", gridTemplateColumns: "26px 1fr 60px", gap: "8px", alignItems: "center" }}>
            <span style={{ fontSize: "11px", fontWeight: 600 }}>{label}</span>
            <div style={{ height: "10px", background: "#eef2f7", borderRadius: "6px", overflow: "hidden" }}>
                <div style={{ height: "100%", width, background: color }} />
            </div>
            <span style={{ fontSize: "11px", color: "#555" }}>{value.toFixed(2)} kWh</span>
        </div>
    );
}

function TrendChart({ meter }: { meter: SmartMeterTelemetryAnalytics }) {
    const trend = meter.trend;
    if (trend.length < 2) {
        return <p style={{ color: "#666", margin: "8px 0" }}>Nedovoljno podataka za trend.</p>;
    }

    const width = 560;
    const height = 240;
    const padding = 20;
    const valuesLoad = trend.map(t => t.loadPercentage);
    const valuesVoltage = trend.map(t => t.voltage).filter((v): v is number => typeof v === "number");
    const maxValue = Math.max(1, ...valuesLoad, ...(valuesVoltage.length ? valuesVoltage : [0]));

    const toPath = (selector: (index: number) => number | null) => {
        let path = "";
        for (let i = 0; i < trend.length; i++) {
            const value = selector(i);
            if (value === null) continue;
            const x = padding + (i / (trend.length - 1)) * (width - padding * 2);
            const y = height - padding - (value / maxValue) * (height - padding * 2);
            path += `${path ? " L" : "M"} ${x} ${y}`;
        }
        return path;
    };

    const loadPath = toPath(i => trend[i].loadPercentage);
    const voltagePath = toPath(i => (typeof trend[i].voltage === "number" ? trend[i].voltage : null));
    const start = new Date(trend[0].timestampUtc).toLocaleTimeString("sr-RS", { hour: "2-digit", minute: "2-digit" });
    const end = new Date(trend[trend.length - 1].timestampUtc).toLocaleTimeString("sr-RS", { hour: "2-digit", minute: "2-digit" });

    return (
        <div>
            <svg viewBox={`0 0 ${width} ${height}`} width="100%" height="240" style={{ background: "#fafafa", borderRadius: "8px" }}>
                <line x1={padding} y1={height - padding} x2={width - padding} y2={height - padding} stroke="#d1d5db" />
                <line x1={padding} y1={padding} x2={padding} y2={height - padding} stroke="#d1d5db" />
                <path d={loadPath} fill="none" stroke="#f97316" strokeWidth="2.5" />
                {voltagePath && <path d={voltagePath} fill="none" stroke="#7c3aed" strokeWidth="2.5" />}
            </svg>
            <div style={{ display: "flex", justifyContent: "space-between", fontSize: "12px", color: "#666", marginTop: "6px" }}>
                <span>{start}</span>
                <span>Load: narandzasto</span>
                <span>Voltage: ljubicasto</span>
                <span>{end}</span>
            </div>
        </div>
    );
}

const pageStyle: React.CSSProperties = { padding: "24px", maxWidth: "1200px", margin: "0 auto" };
const headerStyle: React.CSSProperties = { display: "flex", justifyContent: "space-between", alignItems: "center", gap: "8px" };
const tabsRowStyle: React.CSSProperties = { display: "flex", flexWrap: "wrap", gap: "8px", marginTop: "16px" };
const tabStyle: React.CSSProperties = { padding: "8px 14px", border: "1px solid #d1d5db", borderRadius: "20px", background: "#fff", cursor: "pointer" };
const activeTabStyle: React.CSSProperties = { ...tabStyle, border: "1px solid #2563eb", color: "#fff", background: "#2563eb" };
const cardStyle: React.CSSProperties = { border: "1px solid #e5e7eb", borderRadius: "10px", padding: "16px", backgroundColor: "#fff" };
const statusGridStyle: React.CSSProperties = { display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(160px, 1fr))", gap: "10px", marginBottom: "14px" };
const statusCardStyle: React.CSSProperties = { border: "1px solid #e5e7eb", borderRadius: "8px", padding: "10px", display: "flex", flexDirection: "column" };
const chartsGridStyle: React.CSSProperties = { display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(360px, 1fr))", gap: "12px" };
const chartCardStyle: React.CSSProperties = { border: "1px solid #e5e7eb", borderRadius: "8px", padding: "12px" };
const chartTitleStyle: React.CSSProperties = { margin: "0 0 10px", fontSize: "15px" };
const btnSecondary: React.CSSProperties = { backgroundColor: "#6b7280", color: "#fff", border: "none", padding: "8px 16px", borderRadius: "6px", cursor: "pointer", fontWeight: "bold", fontSize: "14px" };

export default TelemetryAnalyticsPage;
