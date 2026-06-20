import { useEffect, useState } from "react";
import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Grid,
    Paper,
    TextField,
    Typography,
} from "@mui/material";
import SaveRoundedIcon from "@mui/icons-material/SaveRounded";
import AdminRoute from "../../components/admin/AdminRoute";
import PageShell from "../../components/layout/PageShell";
import { getTariffModel, saveTariffModel } from "../../api_services/admin/AdminAPIService";
import type { TariffModel } from "../../types/admin/TariffModel";

const zoneMeta = {
    green: { label: "Zelena", color: "#16a34a" },
    blue: { label: "Plava", color: "#2563eb" },
    red: { label: "Crvena", color: "#dc2626" },
} as const;

function TariffModelsPageContent() {
    const [model, setModel] = useState<TariffModel | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        void loadModel();
    }, []);

    const loadModel = async () => {
        setIsLoading(true);
        setError(null);
        try {
            setModel(await getTariffModel());
        } catch {
            setError("Neuspesno ucitavanje tarifnog modela.");
        } finally {
            setIsLoading(false);
        }
    };

    const updateNumber = (path: string, value: number) => {
        if (!model) return;
        setModel((prev) => {
            if (!prev) return prev;
            const next = structuredClone(prev);
            const keys = path.split(".");
            let current: Record<string, unknown> = next as unknown as Record<string, unknown>;
            for (let i = 0; i < keys.length - 1; i++) {
                current = current[keys[i]] as Record<string, unknown>;
            }
            current[keys[keys.length - 1]] = value;
            return next;
        });
    };

    const handleSave = async () => {
        if (!model) return;
        setIsSaving(true);
        setMessage(null);
        setError(null);
        try {
            setModel(await saveTariffModel(model));
            setMessage("Tarifni model je sacuvan.");
        } catch {
            setError("Neuspesno cuvanje tarifnog modela.");
        } finally {
            setIsSaving(false);
        }
    };

    return (
        <PageShell
            title="Tarifni modeli"
            subtitle="Definisanje cena VT/NT i pragova za zelenu, plavu i crvenu zonu potrosnje."
        >
            {isLoading && (
                <Box sx={{ display: "flex", justifyContent: "center", py: 6 }}>
                    <CircularProgress />
                </Box>
            )}
            {error && <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{error}</Alert>}
            {message && <Alert severity="success" sx={{ mb: 2, borderRadius: 2 }}>{message}</Alert>}

            {model && (
                <>
                    <Paper elevation={0} sx={{ p: 3, mb: 2.5, border: "1px solid", borderColor: "divider", borderRadius: 3 }}>
                        <Typography variant="h6" sx={{ mb: 2 }}>Pragovi potrosnje po zonama (kWh)</Typography>
                        <Grid container spacing={2}>
                            <Grid size={{ xs: 12, md: 6 }}>
                                <TextField
                                    fullWidth
                                    type="number"
                                    label="Zelena zona (0 do)"
                                    value={model.zoneThresholds.greenMaxKwh}
                                    onChange={(e) => updateNumber("zoneThresholds.greenMaxKwh", Number(e.target.value))}
                                />
                            </Grid>
                            <Grid size={{ xs: 12, md: 6 }}>
                                <TextField
                                    fullWidth
                                    type="number"
                                    label="Plava zona (do)"
                                    value={model.zoneThresholds.blueMaxKwh}
                                    onChange={(e) => updateNumber("zoneThresholds.blueMaxKwh", Number(e.target.value))}
                                />
                            </Grid>
                        </Grid>
                        <Typography variant="body2" color="text.secondary" sx={{ mt: 1.5 }}>
                            Crvena zona: potrosnja iznad {model.zoneThresholds.blueMaxKwh} kWh
                        </Typography>
                    </Paper>

                    {(["green", "blue", "red"] as const).map((zone) => (
                        <Paper
                            key={zone}
                            elevation={0}
                            sx={{
                                p: 3,
                                mb: 2.5,
                                border: "1px solid",
                                borderColor: "divider",
                                borderLeft: `4px solid ${zoneMeta[zone].color}`,
                                borderRadius: 3,
                            }}
                        >
                            <Typography variant="h6" sx={{ mb: 2, color: zoneMeta[zone].color }}>
                                {zoneMeta[zone].label} zona — cene (RSD/kWh)
                            </Typography>
                            <Grid container spacing={2}>
                                <Grid size={{ xs: 12, md: 6 }}>
                                    <TextField
                                        fullWidth
                                        type="number"
                                        slotProps={{ htmlInput: { step: 0.01 } }}
                                        label="VT cena"
                                        value={model.zonePrices[zone].vtPriceRsd}
                                        onChange={(e) => updateNumber(`zonePrices.${zone}.vtPriceRsd`, Number(e.target.value))}
                                    />
                                </Grid>
                                <Grid size={{ xs: 12, md: 6 }}>
                                    <TextField
                                        fullWidth
                                        type="number"
                                        slotProps={{ htmlInput: { step: 0.01 } }}
                                        label="NT cena"
                                        value={model.zonePrices[zone].ntPriceRsd}
                                        onChange={(e) => updateNumber(`zonePrices.${zone}.ntPriceRsd`, Number(e.target.value))}
                                    />
                                </Grid>
                            </Grid>
                        </Paper>
                    ))}

                    <Paper elevation={0} sx={{ p: 3, mb: 3, border: "1px solid", borderColor: "divider", borderRadius: 3 }}>
                        <Typography variant="h6" sx={{ mb: 2 }}>Fiksni troskovi</Typography>
                        <Grid container spacing={2}>
                            <Grid size={{ xs: 12, md: 6 }}>
                                <TextField
                                    fullWidth
                                    type="number"
                                    label="Obracunska snaga (RSD po kW)"
                                    value={model.fixedCosts.billingPowerFeeRsd}
                                    onChange={(e) => updateNumber("fixedCosts.billingPowerFeeRsd", Number(e.target.value))}
                                />
                            </Grid>
                            <Grid size={{ xs: 12, md: 6 }}>
                                <TextField
                                    fullWidth
                                    type="number"
                                    label="Trosak snabdevaca (RSD)"
                                    value={model.fixedCosts.supplierCostRsd}
                                    onChange={(e) => updateNumber("fixedCosts.supplierCostRsd", Number(e.target.value))}
                                />
                            </Grid>
                        </Grid>
                    </Paper>

                    <Button
                        variant="contained"
                        size="large"
                        startIcon={<SaveRoundedIcon />}
                        disabled={isSaving}
                        onClick={() => void handleSave()}
                        sx={{ background: "linear-gradient(135deg, #0ea5e9, #6366f1)" }}
                    >
                        {isSaving ? "Cuvanje..." : "Sacuvaj tarifni model"}
                    </Button>
                </>
            )}
        </PageShell>
    );
}

function TariffModelsPage() {
    return (
        <AdminRoute>
            <TariffModelsPageContent />
        </AdminRoute>
    );
}

export default TariffModelsPage;
