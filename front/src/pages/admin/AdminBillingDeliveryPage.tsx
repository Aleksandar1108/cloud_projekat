import { useEffect, useState } from "react";
import {
    Alert,
    Box,
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
import AdminRoute from "../../components/admin/AdminRoute";
import PageShell from "../../components/layout/PageShell";
import StatCard from "../../components/layout/StatCard";
import {
    getBillingDeliveryStats,
    getEmailDeliveryLogs,
} from "../../api_services/admin/AdminAPIService";
import type { BillingDeliveryStats, EmailDeliveryLog } from "../../types/admin/EmailDelivery";

function AdminBillingDeliveryPageContent() {
    const [stats, setStats] = useState<BillingDeliveryStats | null>(null);
    const [logs, setLogs] = useState<EmailDeliveryLog[]>([]);
    const [statusFilter, setStatusFilter] = useState<"All" | "Sent" | "Failed">("All");
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        void loadData();
    }, []);

    const loadData = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const [statsData, logsData] = await Promise.all([
                getBillingDeliveryStats(),
                getEmailDeliveryLogs(),
            ]);
            setStats(statsData);
            setLogs(logsData);
        } catch {
            setError("Neuspesno ucitavanje statistike slanja racuna.");
        } finally {
            setIsLoading(false);
        }
    };

    const filteredLogs =
        statusFilter === "All" ? logs : logs.filter((log) => log.status === statusFilter);

    return (
        <PageShell
            title="Slanje racuna emailom"
            subtitle="Nadzor broja uspesno generisanih i poslatih racuna potrosacima."
            maxWidth="xl"
        >
            {isLoading && (
                <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
                    <CircularProgress />
                </Box>
            )}
            {error && <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{error}</Alert>}

            {stats && (
                <>
                    <Grid container spacing={2} sx={{ mb: 1.5 }}>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Generisani racuni" value={stats.totalGenerated} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Uspesno poslato" value={stats.totalSent} accent="#16a34a" />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Neuspela slanja" value={stats.totalFailed} accent="#dc2626" />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                            <StatCard label="Stopa uspeha" value={`${stats.deliveryRatePercent}%`} accent="#0284c7" />
                        </Grid>
                    </Grid>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                        Poslednji obracun: {stats.lastRunAtUtc ? new Date(stats.lastRunAtUtc).toLocaleString("sr-RS") : "N/A"}
                    </Typography>
                </>
            )}

            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 1.5, gap: 2, flexWrap: "wrap" }}>
                <Typography variant="h6">Pregled poslatih racuna</Typography>
                <TextField
                    select
                    size="small"
                    value={statusFilter}
                    onChange={(e) => setStatusFilter(e.target.value as typeof statusFilter)}
                    sx={{ minWidth: 150 }}
                >
                    <MenuItem value="All">Svi</MenuItem>
                    <MenuItem value="Sent">Poslato</MenuItem>
                    <MenuItem value="Failed">Neuspelo</MenuItem>
                </TextField>
            </Box>

            <TableContainer
                component={Paper}
                elevation={0}
                sx={{ border: "1px solid", borderColor: "divider", borderRadius: 3 }}
            >
                <Table>
                    <TableHead>
                        <TableRow sx={{ bgcolor: "#f8fafc" }}>
                            <TableCell>Vreme</TableCell>
                            <TableCell>Racun</TableCell>
                            <TableCell>Email</TableCell>
                            <TableCell>Period</TableCell>
                            <TableCell>Status</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {filteredLogs.length === 0 && (
                            <TableRow>
                                <TableCell colSpan={5} sx={{ color: "text.secondary" }}>
                                    Nema podataka za izabrani filter.
                                </TableCell>
                            </TableRow>
                        )}
                        {filteredLogs.map((log) => (
                            <TableRow key={log.id} hover>
                                <TableCell>{new Date(log.sentAtUtc).toLocaleString("sr-RS")}</TableCell>
                                <TableCell>{log.invoiceId}</TableCell>
                                <TableCell>{log.ownerEmail}</TableCell>
                                <TableCell>{log.period}</TableCell>
                                <TableCell>
                                    <Chip
                                        size="small"
                                        label={log.status === "Sent" ? "Poslat" : "Neuspelo"}
                                        color={log.status === "Sent" ? "success" : "error"}
                                        variant="outlined"
                                    />
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </PageShell>
    );
}

function AdminBillingDeliveryPage() {
    return (
        <AdminRoute>
            <AdminBillingDeliveryPageContent />
        </AdminRoute>
    );
}

export default AdminBillingDeliveryPage;
