import { useEffect, useMemo, useState } from "react";
import {
    Alert,
    Box,
    Button,
    Chip,
    CircularProgress,
    Grid,
    MenuItem,
    Paper,
    TextField,
    Typography,
} from "@mui/material";
import WarningAmberRoundedIcon from "@mui/icons-material/WarningAmberRounded";
import BoltRoundedIcon from "@mui/icons-material/BoltRounded";
import WifiOffRoundedIcon from "@mui/icons-material/WifiOffRounded";
import EmailRoundedIcon from "@mui/icons-material/EmailRounded";
import PageShell from "../../components/layout/PageShell";
import {
    deleteConsumptionLimit,
    getConsumptionLimit,
    getProperties,
    getSmartMeters,
    setConsumptionLimit,
} from "../../api_services/properties/PropertyAPIService";
import { getPropertyTelemetryAnalytics } from "../../api_services/telemetry/TelemetryAnalyticsAPIService";
import type { ConsumptionLimit, ConsumptionLimitUnit } from "../../types/consumption/ConsumptionLimit";
import type { Property } from "../../types/property/Property";
import type { SmartMeter } from "../../types/property/SmartMeter";
import type { SmartMeterTelemetryAnalytics } from "../../types/telemetry/TelemetryAnalytics";
import { CRITICAL_VOLTAGE, hasCriticalAlert } from "../../helpers/alerts/emergencyAlerts";

type MeterWithContext = {
    property: Property;
    meter: SmartMeter;
    limit: ConsumptionLimit | null;
    telemetry: SmartMeterTelemetryAnalytics | null;
};

type LimitForm = {
    unit: ConsumptionLimitUnit;
    limitValue: string;
};

function EmergencyAlertsPage() {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [message, setMessage] = useState<string | null>(null);
    const [meters, setMeters] = useState<MeterWithContext[]>([]);
    const [limitForms, setLimitForms] = useState<Record<string, LimitForm>>({});
    const [submittingId, setSubmittingId] = useState<string | null>(null);

    useEffect(() => {
        void loadData();
    }, []);

    async function loadData() {
        setLoading(true);
        setError(null);
        try {
            const properties = await getProperties();
            const entries: MeterWithContext[] = [];
            const forms: Record<string, LimitForm> = {};

            for (const property of properties) {
                const [propertyMeters, analytics] = await Promise.all([
                    getSmartMeters(property.id),
                    getPropertyTelemetryAnalytics(property.id).catch(() => null),
                ]);

                for (const meter of propertyMeters) {
                    const limit = await getConsumptionLimit(property.id, meter.id).catch(() => null);
                    const telemetry =
                        analytics?.meters.find((m) => m.smartMeterId === meter.id) ?? null;

                    entries.push({ property, meter, limit, telemetry });
                    forms[meter.id] = {
                        unit: limit?.unit ?? "Kwh",
                        limitValue: limit ? String(limit.limitValue) : "",
                    };
                }
            }

            setMeters(entries);
            setLimitForms(forms);
        } catch {
            setError("Greška pri učitavanju podataka za hitna upozorenja.");
        } finally {
            setLoading(false);
        }
    }

    const criticalCount = useMemo(
        () => meters.filter((entry) => entry.telemetry && hasCriticalAlert(entry.telemetry)).length,
        [meters]
    );

    async function handleSaveLimit(propertyId: string, meterId: string) {
        const form = limitForms[meterId];
        if (!form) return;

        const value = Number(form.limitValue);
        if (!Number.isFinite(value) || value <= 0) {
            setError("Unesite validan limit veći od nule.");
            return;
        }

        setSubmittingId(meterId);
        setError(null);
        setMessage(null);
        try {
            await setConsumptionLimit(propertyId, meterId, { unit: form.unit, limitValue: value });
            setMessage("Limit potrošnje je sačuvan.");
            await loadData();
        } catch {
            setError("Greška pri čuvanju limita potrošnje.");
        } finally {
            setSubmittingId(null);
        }
    }

    async function handleClearLimit(propertyId: string, meterId: string) {
        setSubmittingId(meterId);
        setError(null);
        setMessage(null);
        try {
            await deleteConsumptionLimit(propertyId, meterId);
            setLimitForms((prev) => ({
                ...prev,
                [meterId]: { unit: prev[meterId]?.unit ?? "Kwh", limitValue: "" },
            }));
            setMessage("Limit potrošnje je uklonjen.");
            await loadData();
        } catch {
            setError("Greška pri uklanjanju limita potrošnje.");
        } finally {
            setSubmittingId(null);
        }
    }

    return (
        <PageShell
            title="Hitna upozorenja"
            subtitle="Sistem u realnom vremenu prati telemetriju i šalje obaveštenja kada se detektuje pad napona, prekid rada uređaja ili prekoračenje limita potrošnje."
            maxWidth="xl"
        >
            {loading && (
                <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
                    <CircularProgress />
                </Box>
            )}

            {error && <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{error}</Alert>}
            {message && <Alert severity="success" sx={{ mb: 2, borderRadius: 2 }}>{message}</Alert>}

            {!loading && (
                <>
                    <Grid container spacing={2} sx={{ mb: 3 }}>
                        <Grid size={{ xs: 12, md: 4 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 2.5,
                                    height: "100%",
                                    borderRadius: 3,
                                    border: "1px solid rgba(148, 163, 184, 0.25)",
                                }}
                            >
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 1.5 }}>
                                    <BoltRoundedIcon sx={{ color: "#dc2626" }} />
                                    <Typography variant="h6" sx={{ fontSize: "1rem" }}>
                                        Pad napona
                                    </Typography>
                                </Box>
                                <Typography variant="body2" color="text.secondary" sx={{ lineHeight: 1.7 }}>
                                    Ako napon padne ispod {CRITICAL_VOLTAGE}V, sistem generiše kritično upozorenje
                                    administratoru mreže.
                                </Typography>
                            </Paper>
                        </Grid>

                        <Grid size={{ xs: 12, md: 4 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 2.5,
                                    height: "100%",
                                    borderRadius: 3,
                                    border: "1px solid rgba(148, 163, 184, 0.25)",
                                }}
                            >
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 1.5 }}>
                                    <WifiOffRoundedIcon sx={{ color: "#b45309" }} />
                                    <Typography variant="h6" sx={{ fontSize: "1rem" }}>
                                        Prekid rada uređaja
                                    </Typography>
                                </Box>
                                <Typography variant="body2" color="text.secondary" sx={{ lineHeight: 1.7 }}>
                                    Uređaj koji prestane da šalje podatke nakon definisanog perioda automatski
                                    obaveštava administratora mreže.
                                </Typography>
                            </Paper>
                        </Grid>

                        <Grid size={{ xs: 12, md: 4 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 2.5,
                                    height: "100%",
                                    borderRadius: 3,
                                    border: "1px solid rgba(148, 163, 184, 0.25)",
                                }}
                            >
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 1.5 }}>
                                    <EmailRoundedIcon sx={{ color: "#0284c7" }} />
                                    <Typography variant="h6" sx={{ fontSize: "1rem" }}>
                                        Limit potrošnje
                                    </Typography>
                                </Box>
                                <Typography variant="body2" color="text.secondary" sx={{ lineHeight: 1.7 }}>
                                    Postavite lični limit u kWh ili RSD. Pri prvom prekoračenju u mesecu dobijate
                                    email obaveštenje.
                                </Typography>
                            </Paper>
                        </Grid>
                    </Grid>

                    {criticalCount > 0 && (
                        <Alert
                            severity="warning"
                            icon={<WarningAmberRoundedIcon />}
                            sx={{ mb: 3, borderRadius: 2 }}
                        >
                            Trenutno postoji {criticalCount} brojilo sa kritičnim stanjem (nizak napon ili offline).
                        </Alert>
                    )}

                    <Typography variant="h5" sx={{ mb: 0.5 }}>
                        Limit potrošnje po brojilu
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 2.5 }}>
                        Svaki korisnik ima sopstvena podešavanja. Ako limit nije postavljen, email se ne šalje.
                    </Typography>

                    {meters.length === 0 ? (
                        <Paper
                            elevation={0}
                            sx={{
                                p: 3,
                                borderRadius: 3,
                                border: "1px solid rgba(148, 163, 184, 0.25)",
                            }}
                        >
                            <Typography color="text.secondary">
                                Nemate registrovanih brojila. Dodajte objekat i brojilo da biste podesili limite.
                            </Typography>
                        </Paper>
                    ) : (
                        <Box sx={{ display: "grid", gap: 2 }}>
                            {meters.map(({ property, meter, limit, telemetry }) => {
                                const voltage = telemetry?.status.currentVoltage;
                                const isLowVoltage = typeof voltage === "number" && voltage < CRITICAL_VOLTAGE;
                                const isOffline = telemetry?.status.isOnline === false;
                                const form = limitForms[meter.id] ?? { unit: "Kwh" as ConsumptionLimitUnit, limitValue: "" };

                                return (
                                    <Paper
                                        key={meter.id}
                                        elevation={0}
                                        sx={{
                                            p: 2.5,
                                            borderRadius: 3,
                                            border: "1px solid rgba(148, 163, 184, 0.25)",
                                        }}
                                    >
                                        <Box
                                            sx={{
                                                display: "flex",
                                                flexWrap: "wrap",
                                                justifyContent: "space-between",
                                                gap: 1.5,
                                                mb: 2,
                                            }}
                                        >
                                            <Box>
                                                <Typography variant="h6" sx={{ fontSize: "1.05rem" }}>
                                                    {meter.label}
                                                </Typography>
                                                <Typography variant="body2" color="text.secondary">
                                                    {property.name} · {meter.pairingStatus === "Paired" ? "Upareno" : "Neupareno"}
                                                </Typography>
                                            </Box>

                                            <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1 }}>
                                                {typeof voltage === "number" && (
                                                    <Chip
                                                        size="small"
                                                        label={`Napon: ${voltage.toFixed(1)} V`}
                                                        color={isLowVoltage ? "error" : "default"}
                                                    />
                                                )}
                                                {telemetry && (
                                                    <Chip
                                                        size="small"
                                                        label={telemetry.status.isOnline ? "Online" : "Offline"}
                                                        color={isOffline ? "warning" : "success"}
                                                    />
                                                )}
                                                {limit && (
                                                    <Chip
                                                        size="small"
                                                        label={`Aktivan limit: ${limit.limitValue} ${limit.unit === "Kwh" ? "kWh" : "RSD"}`}
                                                        color="info"
                                                    />
                                                )}
                                            </Box>
                                        </Box>

                                        <Box
                                            sx={{
                                                display: "flex",
                                                flexWrap: "wrap",
                                                gap: 1.5,
                                                alignItems: "center",
                                            }}
                                        >
                                            <TextField
                                                select
                                                label="Jedinica"
                                                size="small"
                                                value={form.unit}
                                                onChange={(e) =>
                                                    setLimitForms((prev) => ({
                                                        ...prev,
                                                        [meter.id]: {
                                                            unit: e.target.value as ConsumptionLimitUnit,
                                                            limitValue: prev[meter.id]?.limitValue ?? "",
                                                        },
                                                    }))
                                                }
                                                sx={{ minWidth: 120 }}
                                            >
                                                <MenuItem value="Kwh">kWh</MenuItem>
                                                <MenuItem value="Rsd">RSD</MenuItem>
                                            </TextField>

                                            <TextField
                                                label="Limit"
                                                size="small"
                                                type="number"
                                                slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                                                placeholder="npr. 350"
                                                value={form.limitValue}
                                                onChange={(e) =>
                                                    setLimitForms((prev) => ({
                                                        ...prev,
                                                        [meter.id]: {
                                                            unit: prev[meter.id]?.unit ?? "Kwh",
                                                            limitValue: e.target.value,
                                                        },
                                                    }))
                                                }
                                                sx={{ minWidth: 160 }}
                                            />

                                            <Button
                                                variant="contained"
                                                disabled={submittingId === meter.id}
                                                onClick={() => void handleSaveLimit(property.id, meter.id)}
                                            >
                                                {submittingId === meter.id ? "Čuvanje..." : "Sačuvaj limit"}
                                            </Button>

                                            <Button
                                                variant="outlined"
                                                color="inherit"
                                                disabled={submittingId === meter.id || !limit}
                                                onClick={() => void handleClearLimit(property.id, meter.id)}
                                            >
                                                Ukloni limit
                                            </Button>
                                        </Box>
                                    </Paper>
                                );
                            })}
                        </Box>
                    )}
                </>
            )}
        </PageShell>
    );
}

export default EmergencyAlertsPage;
