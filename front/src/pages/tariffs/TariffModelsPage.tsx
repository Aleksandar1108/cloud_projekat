import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { TariffModelAPIService } from "../../api_services/tariff/TariffModelAPIService";
import type { TariffModel, TariffModelInput } from "../../models/tariff/TariffModel";

const emptyForm: TariffModelInput = {
    name: "",
    isActive: false,
    greenZoneVtPrice: 0,
    greenZoneNtPrice: 0,
    blueZoneVtPrice: 0,
    blueZoneNtPrice: 0,
    redZoneVtPrice: 0,
    redZoneNtPrice: 0,
    networkCostPerKw: 0,
    supplierCost: 0,
    approvedPowerKw: 0,
    greenZoneLimitKwh: 350,
    blueZoneLimitKwh: 1200
};

const numericFields: { key: keyof TariffModelInput; label: string }[] = [
    { key: "greenZoneVtPrice", label: "Zelena zona VT (RSD/kWh)" },
    { key: "greenZoneNtPrice", label: "Zelena zona NT (RSD/kWh)" },
    { key: "blueZoneVtPrice", label: "Plava zona VT (RSD/kWh)" },
    { key: "blueZoneNtPrice", label: "Plava zona NT (RSD/kWh)" },
    { key: "redZoneVtPrice", label: "Crvena zona VT (RSD/kWh)" },
    { key: "redZoneNtPrice", label: "Crvena zona NT (RSD/kWh)" },
    { key: "networkCostPerKw", label: "Mrezarina (RSD/kW)" },
    { key: "supplierCost", label: "Naknada snabdevaca (RSD)" },
    { key: "approvedPowerKw", label: "Odobrena snaga (kW)" },
    { key: "greenZoneLimitKwh", label: "Prag zelene zone (kWh)" },
    { key: "blueZoneLimitKwh", label: "Prag plave zone (kWh)" }
];

const inputStyle: React.CSSProperties = {
    border: "1px solid #d1d5db",
    borderRadius: "8px",
    padding: "8px 10px",
    width: "100%",
    boxSizing: "border-box"
};

function TariffModelsPage() {
    const navigate = useNavigate();

    const [models, setModels] = useState<TariffModel[]>([]);
    const [form, setForm] = useState<TariffModelInput>(emptyForm);
    const [editingId, setEditingId] = useState<number | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [message, setMessage] = useState<string | null>(null);

    const loadModels = async () => {
        setLoading(true);
        const data = await TariffModelAPIService.getAll();
        setModels(data);
        setLoading(false);
    };

    useEffect(() => {
        void loadModels();
    }, []);

    const resetForm = () => {
        setForm(emptyForm);
        setEditingId(null);
    };

    const startEdit = (model: TariffModel) => {
        setEditingId(model.id);
        setForm({
            name: model.name,
            isActive: model.isActive,
            greenZoneVtPrice: model.greenZoneVtPrice,
            greenZoneNtPrice: model.greenZoneNtPrice,
            blueZoneVtPrice: model.blueZoneVtPrice,
            blueZoneNtPrice: model.blueZoneNtPrice,
            redZoneVtPrice: model.redZoneVtPrice,
            redZoneNtPrice: model.redZoneNtPrice,
            networkCostPerKw: model.networkCostPerKw,
            supplierCost: model.supplierCost,
            approvedPowerKw: model.approvedPowerKw,
            greenZoneLimitKwh: model.greenZoneLimitKwh,
            blueZoneLimitKwh: model.blueZoneLimitKwh
        });
        setMessage(null);
        setError(null);
    };

    const handleSubmit = async () => {
        setError(null);
        setMessage(null);

        if (!form.name.trim()) {
            setError("Naziv tarifnog modela je obavezan.");
            return;
        }
        if (form.greenZoneLimitKwh <= 0 || form.blueZoneLimitKwh <= form.greenZoneLimitKwh) {
            setError("Prag plave zone mora biti veci od praga zelene zone (koji mora biti > 0).");
            return;
        }

        const result = editingId === null
            ? await TariffModelAPIService.create(form)
            : await TariffModelAPIService.update(editingId, form);

        if (result.error) {
            setError(result.error);
            return;
        }

        setMessage(editingId === null ? "Tarifni model kreiran." : "Tarifni model azuriran.");
        resetForm();
        await loadModels();
    };

    const handleActivate = async (id: number) => {
        setError(null);
        setMessage(null);
        const result = await TariffModelAPIService.activate(id);
        if (result.error) {
            setError(result.error);
            return;
        }
        setMessage("Tarifni model je aktiviran.");
        await loadModels();
    };

    return (
        <main style={{ width: "100%", maxWidth: "1040px", margin: "0 auto", padding: "24px" }}>
            <section style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "14px" }}>
                <div>
                    <h1 style={{ margin: 0, fontSize: "40px" }}>Tarifni modeli</h1>
                    <p style={{ color: "#6b7280" }}>Upravljanje cenama po zonama i tarifama (administrator naplate).</p>
                </div>
                <button
                    onClick={() => navigate("/")}
                    style={{ background: "#eef2ff", border: "1px solid #dbeafe", borderRadius: "10px", padding: "10px 14px", cursor: "pointer", fontWeight: 700 }}
                >
                    Nazad na dashboard
                </button>
            </section>

            {error && <p style={{ color: "#dc2626" }}>{error}</p>}
            {message && <p style={{ color: "#16a34a" }}>{message}</p>}

            <section style={{ border: "1px solid #e5e7eb", borderRadius: "12px", padding: "16px", marginBottom: "20px" }}>
                <h3 style={{ marginTop: 0 }}>{editingId === null ? "Novi tarifni model" : `Izmena modela #${editingId}`}</h3>

                <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: "12px" }}>
                    <label style={{ gridColumn: "1 / 2", display: "flex", flexDirection: "column", gap: "4px", fontSize: "13px", color: "#374151" }}>
                        Naziv
                        <input
                            value={form.name}
                            onChange={(e) => setForm({ ...form, name: e.target.value })}
                            style={inputStyle}
                        />
                    </label>

                    {numericFields.map((field) => (
                        <label key={field.key} style={{ display: "flex", flexDirection: "column", gap: "4px", fontSize: "13px", color: "#374151" }}>
                            {field.label}
                            <input
                                type="number"
                                value={form[field.key] as number}
                                onChange={(e) => setForm({ ...form, [field.key]: Number(e.target.value) })}
                                style={inputStyle}
                            />
                        </label>
                    ))}

                    <label style={{ display: "flex", alignItems: "center", gap: "8px", fontSize: "14px", color: "#374151" }}>
                        <input
                            type="checkbox"
                            checked={form.isActive}
                            onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
                        />
                        Postavi kao aktivan
                    </label>
                </div>

                <div style={{ display: "flex", gap: "10px", marginTop: "14px" }}>
                    <button
                        onClick={handleSubmit}
                        style={{ backgroundColor: "var(--secondary)", color: "white", border: "none", borderRadius: "8px", padding: "10px 16px", fontWeight: 700, cursor: "pointer" }}
                    >
                        {editingId === null ? "Kreiraj" : "Sacuvaj izmene"}
                    </button>
                    {editingId !== null && (
                        <button
                            onClick={resetForm}
                            style={{ border: "1px solid #d1d5db", borderRadius: "8px", padding: "10px 16px", cursor: "pointer" }}
                        >
                            Otkazi
                        </button>
                    )}
                </div>
            </section>

            <section style={{ border: "1px solid #e5e7eb", borderRadius: "12px", overflow: "hidden" }}>
                <table style={{ width: "100%", borderCollapse: "collapse" }}>
                    <thead style={{ backgroundColor: "#f3f4f6" }}>
                        <tr>
                            <th style={{ textAlign: "left", padding: "10px" }}>Naziv</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Status</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Pragovi (Z / P) kWh</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Zelena VT/NT</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Plava VT/NT</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Crvena VT/NT</th>
                            <th style={{ textAlign: "left", padding: "10px" }}>Akcije</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading && (
                            <tr><td colSpan={7} style={{ padding: "16px", color: "#6b7280" }}>Ucitavanje...</td></tr>
                        )}
                        {!loading && models.length === 0 && (
                            <tr><td colSpan={7} style={{ padding: "16px", color: "#6b7280" }}>Nema definisanih tarifnih modela.</td></tr>
                        )}
                        {models.map((model) => (
                            <tr key={model.id} style={{ borderTop: "1px solid #f3f4f6" }}>
                                <td style={{ padding: "10px" }}>{model.name}</td>
                                <td style={{ padding: "10px" }}>
                                    {model.isActive
                                        ? <span style={{ color: "#16a34a", fontWeight: 700 }}>Aktivan</span>
                                        : <span style={{ color: "#9ca3af" }}>Neaktivan</span>}
                                </td>
                                <td style={{ padding: "10px" }}>{model.greenZoneLimitKwh} / {model.blueZoneLimitKwh}</td>
                                <td style={{ padding: "10px" }}>{model.greenZoneVtPrice} / {model.greenZoneNtPrice}</td>
                                <td style={{ padding: "10px" }}>{model.blueZoneVtPrice} / {model.blueZoneNtPrice}</td>
                                <td style={{ padding: "10px" }}>{model.redZoneVtPrice} / {model.redZoneNtPrice}</td>
                                <td style={{ padding: "10px", display: "flex", gap: "6px" }}>
                                    <button
                                        onClick={() => startEdit(model)}
                                        style={{ background: "#2563eb", color: "white", border: "none", borderRadius: "6px", padding: "6px 10px", cursor: "pointer" }}
                                    >
                                        Izmeni
                                    </button>
                                    {!model.isActive && (
                                        <button
                                            onClick={() => handleActivate(model.id)}
                                            style={{ background: "#16a34a", color: "white", border: "none", borderRadius: "6px", padding: "6px 10px", cursor: "pointer" }}
                                        >
                                            Aktiviraj
                                        </button>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </section>
        </main>
    );
}

export default TariffModelsPage;
