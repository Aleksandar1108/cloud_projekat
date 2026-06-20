import { useEffect, useMemo, useState } from "react";
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
import MockDataBanner from "../../components/layout/MockDataBanner";
import StatCard from "../../components/layout/StatCard";
import { getMeterStatuses, getRealizedPayments } from "../../services/admin/AdminMockService";
import type { MeterNetworkStatus } from "../../types/admin/MeterNetworkStatus";
import type { AdminPayment } from "../../types/admin/AdminPayment";

function AdminNetworkPageContent() {
    const [meters, setMeters] = useState<MeterNetworkStatus[]>([]);
    const [payments, setPayments] = useState<AdminPayment[]>([]);
    const [selectedMeterId, setSelectedMeterId] = useState<string | null>(null);
    const [statusFilter, setStatusFilter] = useState<"All" | "Online" | "Offline">("All");
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const selectedMeter = useMemo(
        () => meters.find((m) => m.deviceId === selectedMeterId) ?? null,
        [meters, selectedMeterId]
    );

    const filteredMeters = useMemo(() => {
        if (statusFilter === "All") return meters;
        return meters.filter((m) => m.connectionStatus === statusFilter);
    }, [meters, statusFilter]);

    const onlineCount = meters.filter((m) => m.connectionStatus === "Online").length;
    const offlineCount = meters.filter((m) => m.connectionStatus === "Offline").length;
    const totalPayments = payments.reduce((sum, p) => sum + p.amountRsd, 0);

    useEffect(() => {
        void loadData();
    }, []);

    const loadData = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const [meterData, paymentData] = await Promise.all([getMeterStatuses(), getRealizedPayments()]);
            setMeters(meterData);
            setPayments(paymentData);
            if (meterData.length > 0) setSelectedMeterId(meterData[0].deviceId);
        } catch {
            setError("Neuspesno ucitavanje podataka o mrezi.");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <PageShell
            title="Status mreze i uplate"
            subtitle="Pregled online/offline statusa brojila i istorije realizovanih uplata."
            maxWidth="xl"
            banner={<MockDataBanner message="Test podaci — prikazani su mock brojila i mock realizovane uplate." />}
        >
            {isLoading && (
                <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
                    <CircularProgress />
                </Box>
            )}
            {error && <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{error}</Alert>}

            <Grid container spacing={2} sx={{ mb: 3 }}>
                <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                    <StatCard label="Ukupno brojila" value={meters.length} />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                    <StatCard label="Online" value={onlineCount} accent="#16a34a" />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                    <StatCard label="Offline" value={offlineCount} accent="#dc2626" />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                    <StatCard label="Realizovane uplate" value={`${totalPayments.toFixed(2)} RSD`} accent="#0284c7" />
                </Grid>
            </Grid>

            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 1.5 }}>
                <Typography variant="h6">Status brojila</Typography>
                <TextField
                    select
                    size="small"
                    value={statusFilter}
                    onChange={(e) => setStatusFilter(e.target.value as typeof statusFilter)}
                    sx={{ minWidth: 160 }}
                >
                    <MenuItem value="All">Svi statusi</MenuItem>
                    <MenuItem value="Online">Online</MenuItem>
                    <MenuItem value="Offline">Offline</MenuItem>
                </TextField>
            </Box>

            <TableContainer
                component={Paper}
                elevation={0}
                sx={{ mb: 2.5, border: "1px solid", borderColor: "divider", borderRadius: 3 }}
            >
                <Table>
                    <TableHead>
                        <TableRow sx={{ bgcolor: "#f8fafc" }}>
                            <TableCell>Status</TableCell>
                            <TableCell>S/N</TableCell>
                            <TableCell>Vlasnik</TableCell>
                            <TableCell>Adresa</TableCell>
                            <TableCell>Potrosnja</TableCell>
                            <TableCell>Poslednji racun</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {filteredMeters.map((meter) => (
                            <TableRow
                                key={meter.deviceId}
                                hover
                                selected={selectedMeterId === meter.deviceId}
                                onClick={() => setSelectedMeterId(meter.deviceId)}
                                sx={{ cursor: "pointer" }}
                            >
                                <TableCell>
                                    <Chip
                                        size="small"
                                        label={meter.connectionStatus}
                                        color={meter.connectionStatus === "Online" ? "success" : "error"}
                                        variant="outlined"
                                    />
                                </TableCell>
                                <TableCell>{meter.serialNumber}</TableCell>
                                <TableCell>{meter.ownerName}</TableCell>
                                <TableCell>{meter.propertyAddress}</TableCell>
                                <TableCell>{meter.currentMonthKwh.toFixed(1)} kWh</TableCell>
                                <TableCell>{meter.lastBillStatus}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            {selectedMeter && (
                <Paper
                    elevation={0}
                    sx={{
                        p: 2.5,
                        mb: 3,
                        border: "1px solid rgba(14, 165, 233, 0.2)",
                        borderRadius: 3,
                        bgcolor: "rgba(14, 165, 233, 0.05)",
                    }}
                >
                    <Typography variant="h6" sx={{ mb: 1.5 }}>
                        Detalji brojila — {selectedMeter.serialNumber}
                    </Typography>
                    <Grid container spacing={1.5}>
                        <Grid size={{ xs: 12, md: 6 }}><Typography variant="body2"><strong>Vlasnik:</strong> {selectedMeter.ownerName}</Typography></Grid>
                        <Grid size={{ xs: 12, md: 6 }}><Typography variant="body2"><strong>Email:</strong> {selectedMeter.ownerEmail}</Typography></Grid>
                        <Grid size={{ xs: 12, md: 6 }}><Typography variant="body2"><strong>Device ID:</strong> {selectedMeter.deviceId}</Typography></Grid>
                        <Grid size={{ xs: 12, md: 6 }}><Typography variant="body2"><strong>Odobrena snaga:</strong> {selectedMeter.approvedPowerKw} kW</Typography></Grid>
                        <Grid size={{ xs: 12, md: 6 }}><Typography variant="body2"><strong>Poslednji signal:</strong> {selectedMeter.lastSeenAtUtc ?? "N/A"}</Typography></Grid>
                        <Grid size={{ xs: 12, md: 6 }}><Typography variant="body2"><strong>Status:</strong> {selectedMeter.connectionStatus}</Typography></Grid>
                    </Grid>
                </Paper>
            )}

            <Typography variant="h6" sx={{ mb: 1.5 }}>Realizovane uplate</Typography>
            <TableContainer
                component={Paper}
                elevation={0}
                sx={{ border: "1px solid", borderColor: "divider", borderRadius: 3 }}
            >
                <Table>
                    <TableHead>
                        <TableRow sx={{ bgcolor: "#f8fafc" }}>
                            <TableCell>Datum</TableCell>
                            <TableCell>Potrosac</TableCell>
                            <TableCell>Racun</TableCell>
                            <TableCell>Period</TableCell>
                            <TableCell>Iznos</TableCell>
                            <TableCell>Metoda</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {payments.map((payment) => (
                            <TableRow key={payment.id} hover>
                                <TableCell>{new Date(payment.paidAtUtc).toLocaleString("sr-RS")}</TableCell>
                                <TableCell>{payment.ownerName}</TableCell>
                                <TableCell>{payment.invoiceId}</TableCell>
                                <TableCell>{payment.period}</TableCell>
                                <TableCell>{payment.amountRsd.toFixed(2)} RSD</TableCell>
                                <TableCell>{payment.paymentMethod}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </PageShell>
    );
}

function AdminNetworkPage() {
    return (
        <AdminRoute>
            <AdminNetworkPageContent />
        </AdminRoute>
    );
}

export default AdminNetworkPage;
