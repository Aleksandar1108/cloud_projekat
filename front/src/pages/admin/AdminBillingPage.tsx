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
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TextField,
    Typography,
} from "@mui/material";
import PlayArrowRoundedIcon from "@mui/icons-material/PlayArrowRounded";
import AdminRoute from "../../components/admin/AdminRoute";
import PageShell from "../../components/layout/PageShell";
import MockDataBanner from "../../components/layout/MockDataBanner";
import StatCard from "../../components/layout/StatCard";
import { getBillingRuns, runMonthlyBilling } from "../../services/admin/AdminMockService";
import type { AdminGeneratedBill, BillingRun } from "../../types/admin/BillingRun";

function AdminBillingPageContent() {
    const now = new Date();
    const [year, setYear] = useState(now.getFullYear());
    const [month, setMonth] = useState(now.getMonth() === 0 ? 12 : now.getMonth());
    const [runs, setRuns] = useState<BillingRun[]>([]);
    const [selectedRunId, setSelectedRunId] = useState<string | null>(null);
    const [selectedBillIndex, setSelectedBillIndex] = useState<number | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isRunning, setIsRunning] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const selectedRun = useMemo(
        () => runs.find((run) => run.id === selectedRunId) ?? runs[0] ?? null,
        [runs, selectedRunId]
    );

    const selectedBill: AdminGeneratedBill | null =
        selectedRun && selectedBillIndex !== null ? selectedRun.bills[selectedBillIndex] ?? null : null;

    useEffect(() => {
        void loadRuns();
    }, []);

    const loadRuns = async () => {
        setIsLoading(true);
        try {
            const data = await getBillingRuns();
            setRuns(data);
            if (data.length > 0) {
                setSelectedRunId(data[0].id);
                setSelectedBillIndex(data[0].bills.length > 0 ? 0 : null);
            }
        } catch {
            setError("Neuspesno ucitavanje istorije obracuna.");
        } finally {
            setIsLoading(false);
        }
    };

    const handleRunBilling = async () => {
        setIsRunning(true);
        setMessage(null);
        setError(null);
        try {
            const run = await runMonthlyBilling(year, month);
            setRuns((prev) => [run, ...prev]);
            setSelectedRunId(run.id);
            setSelectedBillIndex(run.bills.length > 0 ? 0 : null);
            setMessage(
                `Obracun zavrsen: ${run.generatedBills} racuna generisano, ${run.emailsSent} emailova poslato (mock).`
            );
        } catch {
            setError("Neuspesno pokretanje mesecnog obracuna.");
        } finally {
            setIsRunning(false);
        }
    };

    return (
        <PageShell
            title="Automatizovan mesecni obracun"
            subtitle="Simulacija generisanja racuna na osnovu VT/NT potrosnje i tarifnih zona."
            maxWidth="xl"
            banner={
                <MockDataBanner message="Test podaci — obracun se izvrsava lokalno prema mock merenjima i sacuvanom tarifnom modelu." />
            }
        >
            <Paper
                elevation={0}
                sx={{
                    p: 2.5,
                    mb: 3,
                    border: "1px solid",
                    borderColor: "divider",
                    borderRadius: 3,
                    display: "flex",
                    gap: 2,
                    flexWrap: "wrap",
                    alignItems: "end",
                }}
            >
                <TextField
                    type="number"
                    label="Godina"
                    value={year}
                    onChange={(e) => setYear(Number(e.target.value))}
                    sx={{ width: 140 }}
                />
                <TextField
                    type="number"
                    label="Mesec"
                    value={month}
                    slotProps={{ htmlInput: { min: 1, max: 12 } }}
                    onChange={(e) => setMonth(Number(e.target.value))}
                    sx={{ width: 140 }}
                />
                <Button
                    variant="contained"
                    startIcon={isRunning ? <CircularProgress size={18} color="inherit" /> : <PlayArrowRoundedIcon />}
                    disabled={isRunning}
                    onClick={() => void handleRunBilling()}
                    sx={{ bgcolor: "#0f766e", "&:hover": { bgcolor: "#115e59" } }}
                >
                    {isRunning ? "Obracun u toku..." : "Pokreni mesecni obracun"}
                </Button>
            </Paper>

            {isLoading && (
                <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
                    <CircularProgress />
                </Box>
            )}
            {error && <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{error}</Alert>}
            {message && <Alert severity="success" sx={{ mb: 2, borderRadius: 2 }}>{message}</Alert>}

            {selectedRun && (
                <>
                    <Grid container spacing={2} sx={{ mb: 2.5 }}>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Period" value={`${selectedRun.year}-${String(selectedRun.month).padStart(2, "0")}`} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Obradjena brojila" value={selectedRun.processedMeters} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Generisani racuni" value={selectedRun.generatedBills} accent="#0284c7" />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Poslati emailovi" value={selectedRun.emailsSent} accent="#16a34a" />
                        </Grid>
                    </Grid>

                    {runs.length > 1 && (
                        <TextField
                            select
                            fullWidth
                            label="Istorija obracuna"
                            value={selectedRunId ?? ""}
                            onChange={(e) => {
                                setSelectedRunId(e.target.value);
                                const run = runs.find((r) => r.id === e.target.value);
                                setSelectedBillIndex(run && run.bills.length > 0 ? 0 : null);
                            }}
                            sx={{ mb: 2.5, maxWidth: 420 }}
                        >
                            {runs.map((run) => (
                                <MenuItem key={run.id} value={run.id}>
                                    {run.year}-{String(run.month).padStart(2, "0")} ({run.generatedBills} racuna)
                                </MenuItem>
                            ))}
                        </TextField>
                    )}

                    <TableContainer
                        component={Paper}
                        elevation={0}
                        sx={{ mb: 2.5, border: "1px solid", borderColor: "divider", borderRadius: 3 }}
                    >
                        <Table>
                            <TableHead>
                                <TableRow sx={{ bgcolor: "#f8fafc" }}>
                                    <TableCell>Racun</TableCell>
                                    <TableCell>Potrosac</TableCell>
                                    <TableCell>VT / NT</TableCell>
                                    <TableCell>Zone (Z/P/C)</TableCell>
                                    <TableCell>Ukupno</TableCell>
                                    <TableCell>Email</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {selectedRun.bills.length === 0 && (
                                    <TableRow>
                                        <TableCell colSpan={6} sx={{ color: "text.secondary" }}>
                                            Pokreni obracun da bi se generisali racuni.
                                        </TableCell>
                                    </TableRow>
                                )}
                                {selectedRun.bills.map((bill, index) => (
                                    <TableRow
                                        key={bill.invoiceId}
                                        hover
                                        selected={selectedBillIndex === index}
                                        onClick={() => setSelectedBillIndex(index)}
                                        sx={{ cursor: "pointer" }}
                                    >
                                        <TableCell>{bill.invoiceId}</TableCell>
                                        <TableCell>{bill.ownerName}</TableCell>
                                        <TableCell>{bill.vtKwh} / {bill.ntKwh}</TableCell>
                                        <TableCell>{bill.greenZoneKwh} / {bill.blueZoneKwh} / {bill.redZoneKwh}</TableCell>
                                        <TableCell>{bill.totalCostRsd.toFixed(2)} RSD</TableCell>
                                        <TableCell>
                                            <Chip
                                                size="small"
                                                label={bill.emailSent ? "Poslat" : "Neuspelo"}
                                                color={bill.emailSent ? "success" : "error"}
                                                variant="outlined"
                                            />
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>

                    <Paper elevation={0} sx={{ p: 2.5, border: "1px solid", borderColor: "divider", borderRadius: 3 }}>
                        <Typography variant="h6" sx={{ mb: 1.5 }}>Tekstualni racun</Typography>
                        <Box
                            component="pre"
                            sx={{
                                m: 0,
                                p: 2,
                                borderRadius: 2,
                                bgcolor: "#f8fafc",
                                whiteSpace: "pre-wrap",
                                color: "text.secondary",
                                fontFamily: "ui-monospace, Consolas, monospace",
                                fontSize: "0.9rem",
                            }}
                        >
                            {selectedBill ? selectedBill.billText : "Izaberi racun iz tabele."}
                        </Box>
                    </Paper>
                </>
            )}
        </PageShell>
    );
}

function AdminBillingPage() {
    return (
        <AdminRoute>
            <AdminBillingPageContent />
        </AdminRoute>
    );
}

export default AdminBillingPage;
