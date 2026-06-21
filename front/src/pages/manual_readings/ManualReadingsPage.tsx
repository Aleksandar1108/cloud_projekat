import { Navigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { submitManualReading } from "../../api_services/manual_readings/ManualReadingsAPIService";
import { getProperties, getSmartMeters } from "../../api_services/properties/PropertyAPIService";
import type { SmartMeter } from "../../types/property/SmartMeter";

function ManualReadingsPage() {
    const { isAuthenticated, user } = useAuth();
    const [meterName, setMeterName] = useState("");
    const [availableMeters, setAvailableMeters] = useState<SmartMeter[]>([]);
    const [readingKwh, setReadingKwh] = useState("");
    const [readingAtUtc, setReadingAtUtc] = useState(() => new Date().toISOString().slice(0, 16));
    const [file, setFile] = useState<File | null>(null);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const loadMeters = async () => {
        try {
            const properties = await getProperties();
            const meterGroups = await Promise.all(properties.map((property) => getSmartMeters(property.id)));
            const pairedMeters = meterGroups
                .flat()
                .filter((meter) => meter.pairingStatus === "Paired" && meter.deviceUUID);

            setAvailableMeters(pairedMeters);
            if (pairedMeters.length === 1) {
                setMeterName(pairedMeters[0].label);
            }
        } catch {
            setError("Neuspesno ucitavanje brojila.");
        }
    };

    useEffect(() => {
        void loadMeters();
    }, []);

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
                meterName,
                readingKwh: Number(readingKwh),
                readingAtUtc: new Date(readingAtUtc).toISOString(),
                submitterEmail: user?.username ?? "",
                meterImage: file,
            });
            setMessage("Rucni unos je poslat administratoru na odobravanje. Racun ce biti kreiran nakon odobrenja.");
            setReadingKwh("");
            setFile(null);
        } catch (err) {
            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError("Neuspesno slanje manuelnog ocitavanja.");
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <main style={{ width: "100%", maxWidth: "920px", padding: "24px" }}>
            <h1 style={{ fontSize: "40px", marginBottom: "10px" }}>Prijava stanja brojila</h1>
            <p style={{ marginBottom: "16px" }}>
                U slucaju kvara ili prekida komunikacije, posaljite trenutno stanje brojila i jasnu sliku displeja.
            </p>

            <form onSubmit={onSubmit} style={{ display: "grid", gap: "10px", border: "1px solid #e5e7eb", borderRadius: "10px", padding: "14px", textAlign: "left" }}>
                {availableMeters.length > 0 ? (
                    <select
                        value={meterName}
                        onChange={(e) => setMeterName(e.target.value)}
                        required
                        style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 12px" }}
                    >
                        <option value="">Izaberite naziv brojila</option>
                        {availableMeters.map((meter) => (
                            <option key={meter.id} value={meter.label}>
                                {meter.label}
                            </option>
                        ))}
                    </select>
                ) : (
                    <input
                        value={meterName}
                        onChange={(e) => setMeterName(e.target.value)}
                        placeholder="Naziv brojila"
                        required
                        style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 12px" }}
                    />
                )}
                <input
                    value={readingKwh}
                    onChange={(e) => setReadingKwh(e.target.value)}
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="Ocitano stanje brojila (kWh)"
                    required
                    style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 12px" }}
                />
                <input
                    value={readingAtUtc}
                    onChange={(e) => setReadingAtUtc(e.target.value)}
                    type="datetime-local"
                    required
                    style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 12px" }}
                />
                <input onChange={(e) => setFile(e.target.files?.[0] ?? null)} type="file" accept="image/*" required />
                <button type="submit" disabled={isSubmitting} style={{ backgroundColor: "var(--secondary)", color: "white", border: "none", borderRadius: "8px", padding: "10px", cursor: isSubmitting ? "not-allowed" : "pointer" }}>
                    {isSubmitting ? "Slanje..." : "Posalji rucno ocitavanje"}
                </button>
            </form>

            {message && <p style={{ color: "#0f766e", marginTop: "10px" }}>{message}</p>}
            {error && <p style={{ color: "#dc2626", marginTop: "10px" }}>{error}</p>}
        </main>
    );
}

export default ManualReadingsPage;
