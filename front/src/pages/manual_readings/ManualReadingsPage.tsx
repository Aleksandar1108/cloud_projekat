import { Navigate } from "react-router-dom";
import { useEffect, useMemo, useState } from "react";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { approveManualReading, getManualReadings, submitManualReading } from "../../api_services/manual_readings/ManualReadingsAPIService";
import type { ManualReading } from "../../types/manual_readings/ManualReading";

function ManualReadingsPage() {
    const { isAuthenticated, user } = useAuth();
    const [deviceId, setDeviceId] = useState("");
    const [readingKwh, setReadingKwh] = useState("");
    const [readingAtUtc, setReadingAtUtc] = useState(() => new Date().toISOString().slice(0, 16));
    const [file, setFile] = useState<File | null>(null);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [pendingItems, setPendingItems] = useState<ManualReading[]>([]);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const isAdmin = useMemo(() => {
        const role = String(user?.role ?? "").toLowerCase();
        return role === "admin" || role === "sysadmin";
    }, [user?.role]);

    const loadPending = async () => {
        if (!isAdmin) return;
        const data = await getManualReadings("Pending");
        setPendingItems(data);
    };

    useEffect(() => {
        void loadPending();
    }, [isAdmin]);

    if (!isAuthenticated) return <Navigate to="/login" />;

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!file) {
            setError("Slika displeja je obavezna.");
            return;
        }

        setError(null);
        setMessage(null);
        setIsSubmitting(true);
        try {
            await submitManualReading({
                deviceId,
                readingKwh: Number(readingKwh),
                readingAtUtc: new Date(readingAtUtc).toISOString(),
                submitterEmail: user?.username ?? "",
                meterImage: file
            });
            setMessage("Rucni unos je sacuvan u statusu Pending.");
            setDeviceId("");
            setReadingKwh("");
            setFile(null);
            if (isAdmin) await loadPending();
        } catch {
            setError("Neuspesno slanje manuelnog ocitavanja.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const onApprove = async (id: string) => {
        await approveManualReading(id);
        await loadPending();
    };

    return (
        <main style={{ width: "100%", maxWidth: "920px", padding: "24px" }}>
            <h1 style={{ fontSize: "40px", marginBottom: "10px" }}>Prijava stanja brojila</h1>
            <p style={{ marginBottom: "16px" }}>U slucaju kvara, posalji manuelno merenje i sliku displeja.</p>

            <form onSubmit={onSubmit} style={{ display: "grid", gap: "10px", border: "1px solid #e5e7eb", borderRadius: "10px", padding: "14px", textAlign: "left" }}>
                <input value={deviceId} onChange={(e) => setDeviceId(e.target.value)} placeholder="DeviceId" required />
                <input value={readingKwh} onChange={(e) => setReadingKwh(e.target.value)} type="number" min="0" step="0.01" placeholder="Ocitana potrosnja (kWh)" required />
                <input value={readingAtUtc} onChange={(e) => setReadingAtUtc(e.target.value)} type="datetime-local" required />
                <input onChange={(e) => setFile(e.target.files?.[0] ?? null)} type="file" accept="image/*" required />
                <button type="submit" disabled={isSubmitting} style={{ backgroundColor: "var(--secondary)", color: "white", border: "none", borderRadius: "8px", padding: "10px", cursor: isSubmitting ? "not-allowed" : "pointer" }}>
                    {isSubmitting ? "Slanje..." : "Posalji rucno ocitavanje"}
                </button>
            </form>

            {message && <p style={{ color: "#0f766e", marginTop: "10px" }}>{message}</p>}
            {error && <p style={{ color: "#dc2626", marginTop: "10px" }}>{error}</p>}

            {isAdmin && (
                <section style={{ marginTop: "20px", border: "1px solid #e5e7eb", borderRadius: "10px", padding: "14px", textAlign: "left" }}>
                    <h2 style={{ fontSize: "22px", marginTop: 0 }}>Admin odobravanje (Pending)</h2>
                    {pendingItems.length === 0 && <p>Nema pending zahteva.</p>}
                    {pendingItems.map((item) => (
                        <article key={item.id} style={{ borderTop: "1px solid #f3f4f6", paddingTop: "10px", marginTop: "10px" }}>
                            <p><b>Device:</b> {item.deviceId}</p>
                            <p><b>kWh:</b> {item.readingKwh}</p>
                            <p><b>Vreme:</b> {new Date(item.readingAtUtc).toLocaleString()}</p>
                            <p><b>Poslao:</b> {item.submitterEmail}</p>
                            <button onClick={() => void onApprove(item.id)} style={{ backgroundColor: "#0f766e", color: "white", border: "none", borderRadius: "8px", padding: "8px 10px", cursor: "pointer" }}>
                                Odobri unos
                            </button>
                        </article>
                    ))}
                </section>
            )}
        </main>
    );
}

export default ManualReadingsPage;
