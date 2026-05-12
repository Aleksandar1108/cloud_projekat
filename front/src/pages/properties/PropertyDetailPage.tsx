import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../hooks/auth/useAuthHook";
import {
    getPropertyById,
    updateProperty,
    getSmartMeters,
    addSmartMeter,
    updateSmartMeter,
    deleteSmartMeter,
    registerSerialNumber
} from "../../api_services/properties/PropertyAPIService";
import type { Property, UpdatePropertyDto, PropertyType } from "../../types/property/Property";
import type { SmartMeter, AddSmartMeterDto, ConnectionType } from "../../types/property/SmartMeter";

const PROPERTY_TYPES: PropertyType[] = ["Stan", "Kuca", "Vikendica"];
const PROPERTY_TYPE_LABELS: Record<PropertyType, string> = {
    Stan: "Stan", Kuca: "Kuća", Vikendica: "Vikendica"
};
const CONNECTION_TYPE_LABELS: Record<ConnectionType, string> = {
    Monofazni: "Monofazni (6.9 kW)",
    Trofazni: "Trofazni (11.04 kW)"
};

function PropertyDetailPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { isAuthenticated } = useAuth();

    const [property, setProperty] = useState<Property | null>(null);
    const [meters, setMeters] = useState<SmartMeter[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    const [editMode, setEditMode] = useState(false);
    const [editForm, setEditForm] = useState<UpdatePropertyDto>({ name: "", city: "", address: "", propertyType: "Stan" });

    const [showMeterForm, setShowMeterForm] = useState(false);
    const [meterForm, setMeterForm] = useState<AddSmartMeterDto>({ label: "", connectionType: "Monofazni" });
    const [submittingMeter, setSubmittingMeter] = useState(false);

    const [editingMeter, setEditingMeter] = useState<SmartMeter | null>(null);
    const [editMeterForm, setEditMeterForm] = useState<AddSmartMeterDto>({ label: "", connectionType: "Monofazni" });

    const [serialForms, setSerialForms] = useState<Record<string, string>>({});
    const [serialSubmitting, setSerialSubmitting] = useState<string | null>(null);

    useEffect(() => {
        if (!isAuthenticated) { navigate("/login"); return; }
        if (!id) return;
        loadData();
    }, [isAuthenticated, id]);

    async function loadData() {
        try {
            setLoading(true);
            const [prop, meterList] = await Promise.all([
                getPropertyById(id!),
                getSmartMeters(id!)
            ]);
            setProperty(prop);
            setMeters(meterList);
            setEditForm({ name: prop.name, city: prop.city, address: prop.address, description: prop.description, propertyType: prop.propertyType });
        } catch {
            setError("Greška pri učitavanju podataka.");
        } finally {
            setLoading(false);
        }
    }

    async function handleUpdateProperty(e: React.FormEvent) {
        e.preventDefault();
        try {
            await updateProperty(id!, editForm);
            setProperty(prev => prev ? { ...prev, ...editForm } : prev);
            setEditMode(false);
            showSuccess("Objekat je uspešno ažuriran.");
        } catch {
            setError("Greška pri ažuriranju objekta.");
        }
    }

    async function handleAddMeter(e: React.FormEvent) {
        e.preventDefault();
        setSubmittingMeter(true);
        setError(null);
        try {
            const meter = await addSmartMeter(id!, meterForm);
            setMeters(prev => [...prev, meter]);
            setMeterForm({ label: "", connectionType: "Monofazni" });
            setShowMeterForm(false);
            showSuccess("Brojilo je dodato.");
        } catch {
            setError("Greška pri dodavanju brojila.");
        } finally {
            setSubmittingMeter(false);
        }
    }

    async function handleUpdateMeter(e: React.FormEvent) {
        e.preventDefault();
        if (!editingMeter) return;
        try {
            await updateSmartMeter(id!, editingMeter.id, editMeterForm);
            setMeters(prev => prev.map(m => m.id === editingMeter.id
                ? { ...m, ...editMeterForm, maxApprovedPower: editMeterForm.connectionType === "Monofazni" ? 6.9 : 11.04 }
                : m
            ));
            setEditingMeter(null);
            showSuccess("Brojilo je ažurirano.");
        } catch {
            setError("Greška pri ažuriranju brojila.");
        }
    }

    async function handleDeleteMeter(meterId: string) {
        if (!confirm("Obrisati ovo brojilo?")) return;
        try {
            await deleteSmartMeter(id!, meterId);
            setMeters(prev => prev.filter(m => m.id !== meterId));
            showSuccess("Brojilo je obrisano.");
        } catch {
            setError("Greška pri brisanju brojila.");
        }
    }

    async function handleRegisterSerial(meterId: string) {
        const sn = serialForms[meterId]?.trim();
        if (!sn) return;
        setSerialSubmitting(meterId);
        setError(null);
        try {
            await registerSerialNumber(id!, meterId, sn);
            setMeters(prev => prev.map(m => m.id === meterId ? { ...m, serialNumber: sn } : m));
            setSerialForms(prev => ({ ...prev, [meterId]: "" }));
            showSuccess(`Serijski broj registrovan. Uređaj je spreman za uparivanje.`);
        } catch (err: unknown) {
            const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
            setError(msg ?? "Greška pri registraciji serijskog broja.");
        } finally {
            setSerialSubmitting(null);
        }
    }

    function showSuccess(msg: string) {
        setSuccess(msg);
        setTimeout(() => setSuccess(null), 3000);
    }

    if (loading) return <main style={{ padding: "24px" }}><p>Učitavanje...</p></main>;
    if (!property) return <main style={{ padding: "24px" }}><p>Objekat nije pronađen.</p></main>;

    return (
        <main style={{ padding: "24px", maxWidth: "900px", margin: "0 auto" }}>
            <button onClick={() => navigate("/properties")} style={btnSecondary}>
                ← Nazad na objekte
            </button>

            {error && <p style={{ color: "red", marginTop: "12px" }}>{error}</p>}
            {success && <p style={{ color: "green", marginTop: "12px" }}>{success}</p>}

            {/* ── Property Info ───────────────────────────────────── */}
            <div style={{ ...cardStyle, marginTop: "16px" }}>
                {!editMode ? (
                    <>
                        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                            <div>
                                <h2 style={{ margin: "0 0 4px" }}>{property.name}</h2>
                                <span style={badgeStyle}>{PROPERTY_TYPE_LABELS[property.propertyType]}</span>
                                <p style={{ margin: "8px 0 2px" }}>{property.address}, {property.city}</p>
                                {property.description && <p style={{ color: "#666", margin: 0 }}>{property.description}</p>}
                            </div>
                            <button onClick={() => setEditMode(true)} style={btnPrimary}>Izmeni</button>
                        </div>
                    </>
                ) : (
                    <form onSubmit={handleUpdateProperty} style={{ display: "flex", flexDirection: "column", gap: "8px" }}>
                        <h3>Izmena objekta</h3>
                        <label>Naziv*</label>
                        <input style={inputStyle} value={editForm.name} onChange={e => setEditForm({ ...editForm, name: e.target.value })} required />
                        <label>Grad*</label>
                        <input style={inputStyle} value={editForm.city} onChange={e => setEditForm({ ...editForm, city: e.target.value })} required />
                        <label>Adresa*</label>
                        <input style={inputStyle} value={editForm.address} onChange={e => setEditForm({ ...editForm, address: e.target.value })} required />
                        <label>Opis</label>
                        <textarea style={{ ...inputStyle, height: "60px" }} value={editForm.description ?? ""} onChange={e => setEditForm({ ...editForm, description: e.target.value })} />
                        <label>Tip</label>
                        <select style={inputStyle} value={editForm.propertyType} onChange={e => setEditForm({ ...editForm, propertyType: e.target.value as PropertyType })}>
                            {PROPERTY_TYPES.map(t => <option key={t} value={t}>{PROPERTY_TYPE_LABELS[t]}</option>)}
                        </select>
                        <div style={{ display: "flex", gap: "8px" }}>
                            <button type="submit" style={btnPrimary}>Sačuvaj</button>
                            <button type="button" onClick={() => setEditMode(false)} style={btnSecondary}>Otkaži</button>
                        </div>
                    </form>
                )}
            </div>

            {/* ── Smart Meters ────────────────────────────────────── */}
            <div style={{ marginTop: "24px" }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                    <h3>Pametna brojila</h3>
                    <button onClick={() => setShowMeterForm(!showMeterForm)} style={btnPrimary}>
                        {showMeterForm ? "Otkaži" : "+ Dodaj brojilo"}
                    </button>
                </div>

                {showMeterForm && (
                    <form onSubmit={handleAddMeter} style={{ ...formStyle, marginTop: "12px" }}>
                        <h4 style={{ margin: "0 0 8px" }}>Novo brojilo</h4>
                        <label>Oznaka*</label>
                        <input style={inputStyle} value={meterForm.label} onChange={e => setMeterForm({ ...meterForm, label: e.target.value })} required placeholder="npr. Brojilo 1" />
                        <label>Tip priključka*</label>
                        <select style={inputStyle} value={meterForm.connectionType} onChange={e => setMeterForm({ ...meterForm, connectionType: e.target.value as ConnectionType })}>
                            <option value="Monofazni">Monofazni (6.9 kW)</option>
                            <option value="Trofazni">Trofazni (11.04 kW)</option>
                        </select>
                        <label>Napomena</label>
                        <input style={inputStyle} value={meterForm.note ?? ""} onChange={e => setMeterForm({ ...meterForm, note: e.target.value })} placeholder="opciono" />
                        <button type="submit" disabled={submittingMeter} style={btnPrimary}>
                            {submittingMeter ? "Čuvanje..." : "Dodaj brojilo"}
                        </button>
                    </form>
                )}

                {meters.length === 0 ? (
                    <p style={{ color: "#666", marginTop: "12px" }}>Nema dodanih brojila. Kliknite na "+ Dodaj brojilo".</p>
                ) : (
                    <div style={{ display: "grid", gap: "12px", marginTop: "12px" }}>
                        {meters.map(m => (
                            <div key={m.id} style={cardStyle}>
                                {editingMeter?.id === m.id ? (
                                    <form onSubmit={handleUpdateMeter} style={{ display: "flex", flexDirection: "column", gap: "8px" }}>
                                        <label>Oznaka*</label>
                                        <input style={inputStyle} value={editMeterForm.label} onChange={e => setEditMeterForm({ ...editMeterForm, label: e.target.value })} required />
                                        <label>Tip priključka*</label>
                                        <select style={inputStyle} value={editMeterForm.connectionType} onChange={e => setEditMeterForm({ ...editMeterForm, connectionType: e.target.value as ConnectionType })}>
                                            <option value="Monofazni">Monofazni (6.9 kW)</option>
                                            <option value="Trofazni">Trofazni (11.04 kW)</option>
                                        </select>
                                        <label>Napomena</label>
                                        <input style={inputStyle} value={editMeterForm.note ?? ""} onChange={e => setEditMeterForm({ ...editMeterForm, note: e.target.value })} />
                                        <div style={{ display: "flex", gap: "8px" }}>
                                            <button type="submit" style={btnPrimary}>Sačuvaj</button>
                                            <button type="button" onClick={() => setEditingMeter(null)} style={btnSecondary}>Otkaži</button>
                                        </div>
                                    </form>
                                ) : (
                                    <>
                                        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                                            <div>
                                                <strong>{m.label}</strong>
                                                <span style={{ ...badgeStyle, marginLeft: "8px" }}>{CONNECTION_TYPE_LABELS[m.connectionType]}</span>
                                                {m.note && <p style={{ margin: "4px 0 0", color: "#666", fontSize: "13px" }}>{m.note}</p>}
                                                <p style={{ margin: "6px 0 0", fontSize: "13px" }}>
                                                    <strong>Status:</strong>{" "}
                                                    <span style={{ color: m.pairingStatus === "Paired" ? "green" : "#f59e0b", fontWeight: "bold" }}>
                                                        {m.pairingStatus === "Paired" ? "Uparen" : "Neuparen"}
                                                    </span>
                                                </p>
                                                {m.serialNumber && (
                                                    <p style={{ margin: "2px 0 0", fontSize: "13px", color: "#555" }}>
                                                        S/N: <code>{m.serialNumber}</code>
                                                    </p>
                                                )}
                                            </div>
                                            <div style={{ display: "flex", gap: "8px" }}>
                                                <button
                                                    onClick={() => { setEditingMeter(m); setEditMeterForm({ label: m.label, connectionType: m.connectionType, note: m.note }); }}
                                                    style={btnPrimary}
                                                >
                                                    Izmeni
                                                </button>
                                                <button onClick={() => handleDeleteMeter(m.id)} style={btnDanger}>Obriši</button>
                                            </div>
                                        </div>

                                        {m.pairingStatus === "Unpaired" && (
                                            <div style={{ marginTop: "12px", paddingTop: "12px", borderTop: "1px solid #e5e7eb" }}>
                                                <p style={{ margin: "0 0 6px", fontSize: "13px", color: "#555" }}>
                                                    Unesite serijski broj (S/N) sa poleđine uređaja da biste pokrenuli uparivanje:
                                                </p>
                                                <div style={{ display: "flex", gap: "8px" }}>
                                                    <input
                                                        style={{ ...inputStyle, width: "200px" }}
                                                        placeholder="SA-2024-12345"
                                                        value={serialForms[m.id] ?? ""}
                                                        onChange={e => setSerialForms(prev => ({ ...prev, [m.id]: e.target.value }))}
                                                    />
                                                    <button
                                                        onClick={() => handleRegisterSerial(m.id)}
                                                        disabled={serialSubmitting === m.id}
                                                        style={btnPrimary}
                                                    >
                                                        {serialSubmitting === m.id ? "Čuvanje..." : "Registruj S/N"}
                                                    </button>
                                                </div>
                                            </div>
                                        )}
                                    </>
                                )}
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </main>
    );
}

const btnPrimary: React.CSSProperties = { backgroundColor: "var(--secondary, #2563eb)", color: "#fff", border: "none", padding: "8px 16px", borderRadius: "6px", cursor: "pointer", fontWeight: "bold", fontSize: "14px" };
const btnSecondary: React.CSSProperties = { backgroundColor: "#6b7280", color: "#fff", border: "none", padding: "8px 16px", borderRadius: "6px", cursor: "pointer", fontWeight: "bold", fontSize: "14px" };
const btnDanger: React.CSSProperties = { backgroundColor: "#dc2626", color: "#fff", border: "none", padding: "8px 16px", borderRadius: "6px", cursor: "pointer", fontWeight: "bold", fontSize: "14px" };
const cardStyle: React.CSSProperties = { border: "1px solid #e5e7eb", borderRadius: "8px", padding: "16px", backgroundColor: "#fff" };
const formStyle: React.CSSProperties = { border: "1px solid #e5e7eb", borderRadius: "8px", padding: "16px", backgroundColor: "#f9fafb", display: "flex", flexDirection: "column", gap: "8px", maxWidth: "440px" };
const inputStyle: React.CSSProperties = { padding: "8px", borderRadius: "4px", border: "1px solid #d1d5db", fontSize: "14px", width: "100%", boxSizing: "border-box", color: "#111", backgroundColor: "#fff" };
const badgeStyle: React.CSSProperties = { backgroundColor: "#dbeafe", color: "#1d4ed8", padding: "2px 8px", borderRadius: "12px", fontSize: "12px", fontWeight: "bold" };

export default PropertyDetailPage;
