import { Navigate } from "react-router-dom";
import { useEffect, useMemo, useState } from "react";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { approveManualReading, getManualReadings, getManualReadingImageUrl, submitManualReading } from "../../api_services/manual_readings/ManualReadingsAPIService";
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
    const [processedItems, setProcessedItems] = useState<ManualReading[]>([]);
    const [imageUrls, setImageUrls] = useState<Record<string, string>>({});
    const [isSubmitting, setIsSubmitting] = useState(false);

    const isAdmin = useMemo(() => {
        const role = String(user?.role ?? "").toLowerCase();
        return role === "admin" || role === "sysadmin";
    }, [user?.role]);

    const loadImages = async (items: ManualReading[]) => {
        const entries = await Promise.all(
            items.map(async (item) => [item.id, await getManualReadingImageUrl(item.id)] as const)
        );
        setImageUrls((prev) => {
            const next = { ...prev };
            for (const [id, url] of entries) {
                if (url) next[id] = url;
            }
            return next;
        });
    };

    const loadReadings = async () => {
        if (!isAdmin) return;
        const [pending, processed] = await Promise.all([
            getManualReadings("Pending"),
            getManualReadings("Processed")
        ]);
        setPendingItems(pending);
        setProcessedItems(processed);
        void loadImages([...pending, ...processed]);
    };

    useEffect(() => {
        void loadReadings();
        // eslint-disable-next-line react-hooks/exhaustive-deps
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
            if (isAdmin) await loadReadings();
        } catch {
            setError("Neuspesno slanje manuelnog ocitavanja.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const onApprove = async (id: string) => {
        await approveManualReading(id);
        await loadReadings();
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
                        <article key={item.id} style={{ display: "flex", gap: "14px", borderTop: "1px solid #f3f4f6", paddingTop: "10px", marginTop: "10px" }}>
                            {imageUrls[item.id]
                                ? <img src={imageUrls[item.id]} alt="Dokaz ocitavanja" style={{ width: "140px", height: "140px", objectFit: "cover", borderRadius: "8px", border: "1px solid #e5e7eb" }} />
                                : <div style={{ width: "140px", height: "140px", display: "flex", alignItems: "center", justifyContent: "center", borderRadius: "8px", border: "1px dashed #d1d5db", color: "#9ca3af", fontSize: "12px" }}>Bez slike</div>}
                            <div style={{ flex: 1 }}>
                                <p style={{ margin: "2px 0" }}><b>Device:</b> {item.deviceId}</p>
                                <p style={{ margin: "2px 0" }}><b>kWh:</b> {item.readingKwh}</p>
                                <p style={{ margin: "2px 0" }}><b>Vreme:</b> {new Date(item.readingAtUtc).toLocaleString()}</p>
                                <p style={{ margin: "2px 0" }}><b>Poslao:</b> {item.submitterEmail}</p>
                                <button onClick={() => void onApprove(item.id)} style={{ marginTop: "6px", backgroundColor: "#0f766e", color: "white", border: "none", borderRadius: "8px", padding: "8px 10px", cursor: "pointer" }}>
                                    Odobri unos
                                </button>
                            </div>
                        </article>
                    ))}

                    <h2 style={{ fontSize: "22px", marginTop: "24px" }}>Odobreno (Processed)</h2>
                    {processedItems.length === 0 && <p>Jos nema odobrenih ocitavanja.</p>}
                    {processedItems.map((item) => (
                        <article key={item.id} style={{ display: "flex", gap: "14px", borderTop: "1px solid #f3f4f6", paddingTop: "10px", marginTop: "10px", opacity: 0.9 }}>
                            {imageUrls[item.id]
                                ? <img src={imageUrls[item.id]} alt="Dokaz ocitavanja" style={{ width: "100px", height: "100px", objectFit: "cover", borderRadius: "8px", border: "1px solid #e5e7eb" }} />
                                : <div style={{ width: "100px", height: "100px", display: "flex", alignItems: "center", justifyContent: "center", borderRadius: "8px", border: "1px dashed #d1d5db", color: "#9ca3af", fontSize: "12px" }}>Bez slike</div>}
                            <div style={{ flex: 1 }}>
                                <p style={{ margin: "2px 0" }}><b>Device:</b> {item.deviceId}</p>
                                <p style={{ margin: "2px 0" }}><b>kWh:</b> {item.readingKwh}</p>
                                <p style={{ margin: "2px 0" }}><b>Vreme:</b> {new Date(item.readingAtUtc).toLocaleString()}</p>
                                <span style={{ display: "inline-block", marginTop: "4px", color: "#16a34a", fontWeight: 700 }}>Processed</span>
                            </div>
                        </article>
                    ))}
                </section>
            )}
        </main>
    );
}

export default ManualReadingsPage;
