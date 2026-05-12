import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks/auth/useAuthHook";
import {
    getProperties,
    createProperty,
    deleteProperty
} from "../../api_services/properties/PropertyAPIService";
import type { Property, CreatePropertyDto, PropertyType } from "../../types/property/Property";

const PROPERTY_TYPES: PropertyType[] = ["Stan", "Kuca", "Vikendica"];
const PROPERTY_TYPE_LABELS: Record<PropertyType, string> = {
    Stan: "Stan",
    Kuca: "Kuća",
    Vikendica: "Vikendica"
};

function PropertiesPage() {
    const navigate = useNavigate();
    const { isAuthenticated } = useAuth();

    const [properties, setProperties] = useState<Property[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showForm, setShowForm] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    const [form, setForm] = useState<CreatePropertyDto>({
        name: "",
        city: "",
        address: "",
        description: "",
        propertyType: "Stan"
    });

    useEffect(() => {
        if (!isAuthenticated) {
            navigate("/login");
            return;
        }
        loadProperties();
    }, [isAuthenticated]);

    async function loadProperties() {
        try {
            setLoading(true);
            const data = await getProperties();
            setProperties(data);
        } catch {
            setError("Greška pri učitavanju objekata.");
        } finally {
            setLoading(false);
        }
    }

    async function handleCreate(e: React.FormEvent) {
        e.preventDefault();
        setSubmitting(true);
        setError(null);
        try {
            await createProperty(form);
            setForm({ name: "", city: "", address: "", description: "", propertyType: "Stan" });
            setShowForm(false);
            await loadProperties();
        } catch {
            setError("Greška pri dodavanju objekta.");
        } finally {
            setSubmitting(false);
        }
    }

    async function handleDelete(id: string) {
        if (!confirm("Da li ste sigurni da želite da obrišete objekat?")) return;
        try {
            await deleteProperty(id);
            setProperties(prev => prev.filter(p => p.id !== id));
        } catch {
            setError("Greška pri brisanju objekta.");
        }
    }

    return (
        <main style={{ padding: "24px", maxWidth: "900px", margin: "0 auto" }}>
            <h1>Moji objekti</h1>

            <button onClick={() => navigate("/")} style={btnSecondary}>
                ← Nazad na Dashboard
            </button>

            {error && <p style={{ color: "red", marginTop: "12px" }}>{error}</p>}

            <div style={{ marginTop: "16px", marginBottom: "16px" }}>
                <button onClick={() => setShowForm(!showForm)} style={btnPrimary}>
                    {showForm ? "Otkaži" : "+ Dodaj objekat"}
                </button>
            </div>

            {showForm && (
                <form onSubmit={handleCreate} style={formStyle}>
                    <h3>Novi objekat</h3>

                    <label>Naziv*</label>
                    <input
                        style={inputStyle}
                        value={form.name}
                        onChange={e => setForm({ ...form, name: e.target.value })}
                        required
                        placeholder="npr. Moj stan"
                    />

                    <label>Grad*</label>
                    <input
                        style={inputStyle}
                        value={form.city}
                        onChange={e => setForm({ ...form, city: e.target.value })}
                        required
                        placeholder="npr. Novi Sad"
                    />

                    <label>Adresa*</label>
                    <input
                        style={inputStyle}
                        value={form.address}
                        onChange={e => setForm({ ...form, address: e.target.value })}
                        required
                        placeholder="npr. Bulevar oslobođenja 12"
                    />

                    <label>Opis (opciono)</label>
                    <textarea
                        style={{ ...inputStyle, height: "80px" }}
                        value={form.description}
                        onChange={e => setForm({ ...form, description: e.target.value })}
                        placeholder="Kratak opis objekta..."
                    />

                    <label>Tip objekta*</label>
                    <select
                        style={inputStyle}
                        value={form.propertyType}
                        onChange={e => setForm({ ...form, propertyType: e.target.value as PropertyType })}
                    >
                        {PROPERTY_TYPES.map(t => (
                            <option key={t} value={t}>{PROPERTY_TYPE_LABELS[t]}</option>
                        ))}
                    </select>

                    <button type="submit" disabled={submitting} style={btnPrimary}>
                        {submitting ? "Čuvanje..." : "Sačuvaj objekat"}
                    </button>
                </form>
            )}

            {loading ? (
                <p>Učitavanje...</p>
            ) : properties.length === 0 ? (
                <p style={{ color: "#666", marginTop: "24px" }}>Nemate unesenih objekata. Dodajte prvi objekat klikom na dugme iznad.</p>
            ) : (
                <div style={{ display: "grid", gap: "16px", marginTop: "16px" }}>
                    {properties.map(p => (
                        <div key={p.id} style={cardStyle}>
                            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                                <div>
                                    <h3 style={{ margin: "0 0 4px" }}>{p.name}</h3>
                                    <span style={badgeStyle}>{PROPERTY_TYPE_LABELS[p.propertyType]}</span>
                                    <p style={{ margin: "8px 0 4px", color: "#555" }}>
                                        {p.address}, {p.city}
                                    </p>
                                    {p.description && (
                                        <p style={{ margin: 0, color: "#888", fontSize: "14px" }}>{p.description}</p>
                                    )}
                                </div>
                                <div style={{ display: "flex", gap: "8px" }}>
                                    <button
                                        onClick={() => navigate(`/properties/${p.id}`)}
                                        style={btnPrimary}
                                    >
                                        Upravljaj
                                    </button>
                                    <button
                                        onClick={() => handleDelete(p.id)}
                                        style={btnDanger}
                                    >
                                        Obriši
                                    </button>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </main>
    );
}

const btnPrimary: React.CSSProperties = {
    backgroundColor: "var(--secondary, #2563eb)",
    color: "#fff",
    border: "none",
    padding: "8px 16px",
    borderRadius: "6px",
    cursor: "pointer",
    fontWeight: "bold",
    fontSize: "14px"
};

const btnSecondary: React.CSSProperties = {
    backgroundColor: "#6b7280",
    color: "#fff",
    border: "none",
    padding: "8px 16px",
    borderRadius: "6px",
    cursor: "pointer",
    fontWeight: "bold",
    fontSize: "14px"
};

const btnDanger: React.CSSProperties = {
    backgroundColor: "#dc2626",
    color: "#fff",
    border: "none",
    padding: "8px 16px",
    borderRadius: "6px",
    cursor: "pointer",
    fontWeight: "bold",
    fontSize: "14px"
};

const cardStyle: React.CSSProperties = {
    border: "1px solid #e5e7eb",
    borderRadius: "8px",
    padding: "16px",
    backgroundColor: "#fff"
};

const formStyle: React.CSSProperties = {
    border: "1px solid #e5e7eb",
    borderRadius: "8px",
    padding: "20px",
    backgroundColor: "#f9fafb",
    display: "flex",
    flexDirection: "column",
    gap: "8px",
    maxWidth: "480px",
    marginBottom: "16px"
};

const inputStyle: React.CSSProperties = {
    padding: "8px",
    borderRadius: "4px",
    border: "1px solid #d1d5db",
    fontSize: "14px",
    width: "100%",
    boxSizing: "border-box",
    color: "#111",
    backgroundColor: "#fff"
};

const badgeStyle: React.CSSProperties = {
    backgroundColor: "#dbeafe",
    color: "#1d4ed8",
    padding: "2px 8px",
    borderRadius: "12px",
    fontSize: "12px",
    fontWeight: "bold"
};

export default PropertiesPage;
