import { useEffect, useState } from "react";
import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Grid,
    Paper,
    Stack,
    Typography,
} from "@mui/material";
import CheckCircleRoundedIcon from "@mui/icons-material/CheckCircleRounded";
import CancelRoundedIcon from "@mui/icons-material/CancelRounded";
import AdminRoute from "../../components/admin/AdminRoute";
import PageShell from "../../components/layout/PageShell";
import type { ManualReading } from "../../types/manual_readings/ManualReading";
import {
    approveManualReadingAdmin,
    fetchManualReadingImageBlob,
    getPendingManualReadings,
    rejectManualReadingAdmin,
} from "../../api_services/admin/AdminManualReadingsAPIService";

function PendingReadingCard({
    item,
    onApprove,
    onReject,
    isProcessing,
}: {
    item: ManualReading;
    onApprove: (id: string) => Promise<void>;
    onReject: (id: string) => Promise<void>;
    isProcessing: boolean;
}) {
    const [imageUrl, setImageUrl] = useState<string | null>(null);
    const [imageError, setImageError] = useState(false);

    useEffect(() => {
        let active = true;
        let objectUrl: string | null = null;

        void fetchManualReadingImageBlob(item.id)
            .then((blob) => {
                if (!active) return;
                objectUrl = URL.createObjectURL(blob);
                setImageUrl(objectUrl);
            })
            .catch(() => {
                if (active) setImageError(true);
            });

        return () => {
            active = false;
            if (objectUrl) URL.revokeObjectURL(objectUrl);
        };
    }, [item.id]);

    return (
        <Paper elevation={0} sx={{ p: 3, borderRadius: 3, border: "1px solid", borderColor: "divider" }}>
            <Grid container spacing={3}>
                <Grid size={{ xs: 12, md: 5 }}>
                    <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 1 }}>
                        Slika displeja brojila
                    </Typography>
                    <Box
                        sx={{
                            borderRadius: 2,
                            overflow: "hidden",
                            border: "1px solid",
                            borderColor: "divider",
                            minHeight: 220,
                            display: "grid",
                            placeItems: "center",
                            bgcolor: "#f8fafc",
                        }}
                    >
                        {imageUrl ? (
                            <Box
                                component="img"
                                src={imageUrl}
                                alt="Slika displeja brojila"
                                sx={{ width: "100%", maxHeight: 320, objectFit: "contain" }}
                            />
                        ) : imageError ? (
                            <Typography color="error">Slika nije dostupna.</Typography>
                        ) : (
                            <CircularProgress size={28} />
                        )}
                    </Box>
                </Grid>

                <Grid size={{ xs: 12, md: 7 }}>
                    <Stack spacing={1.2}>
                        <Typography variant="h6">{item.meterName || item.deviceId}</Typography>
                        <Typography><b>Ocitano stanje:</b> {item.readingKwh} kWh</Typography>
                        <Typography><b>Vreme ocitavanja:</b> {new Date(item.readingAtUtc).toLocaleString("sr-RS")}</Typography>
                        <Typography><b>Poslao:</b> {item.submitterEmail}</Typography>
                        <Typography><b>Status:</b> Pending</Typography>
                    </Stack>

                    <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5} sx={{ mt: 3 }}>
                        <Button
                            variant="contained"
                            color="success"
                            startIcon={<CheckCircleRoundedIcon />}
                            disabled={isProcessing}
                            onClick={() => void onApprove(item.id)}
                        >
                            Odobri i kreiraj racun
                        </Button>
                        <Button
                            variant="outlined"
                            color="error"
                            startIcon={<CancelRoundedIcon />}
                            disabled={isProcessing}
                            onClick={() => void onReject(item.id)}
                        >
                            Odbij
                        </Button>
                    </Stack>
                </Grid>
            </Grid>
        </Paper>
    );
}

function AdminManualReadingsPageContent() {
    const [items, setItems] = useState<ManualReading[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [processingId, setProcessingId] = useState<string | null>(null);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const loadItems = async () => {
        setIsLoading(true);
        setError(null);
        try {
            setItems(await getPendingManualReadings());
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno ucitavanje zahteva.");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        void loadItems();
    }, []);

    const handleApprove = async (id: string) => {
        setProcessingId(id);
        setMessage(null);
        setError(null);
        try {
            await approveManualReadingAdmin(id);
            setItems((prev) => prev.filter((item) => item.id !== id));
            setMessage("Ocitavanje je odobreno i racun je kreiran za korisnika.");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno odobravanje.");
        } finally {
            setProcessingId(null);
        }
    };

    const handleReject = async (id: string) => {
        const confirmed = window.confirm("Da li ste sigurni da zelite da odbijete ovo ocitavanje?");
        if (!confirmed) return;

        setProcessingId(id);
        setMessage(null);
        setError(null);
        try {
            await rejectManualReadingAdmin(id);
            setItems((prev) => prev.filter((item) => item.id !== id));
            setMessage("Ocitavanje je odbijeno.");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno odbijanje.");
        } finally {
            setProcessingId(null);
        }
    };

    return (
        <PageShell
            title="Odobravanje rucnih ocitavanja"
            subtitle="Pregledajte sliku displeja, uporedite sa unetim stanjem i odobrite ili odbijte zahtev"
        >
            {message && <Alert severity="success" sx={{ mb: 2 }}>{message}</Alert>}
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

            {isLoading ? (
                <Box sx={{ display: "grid", placeItems: "center", py: 8 }}>
                    <CircularProgress />
                </Box>
            ) : items.length === 0 ? (
                <Paper elevation={0} sx={{ p: 4, textAlign: "center", borderRadius: 3, border: "1px solid", borderColor: "divider" }}>
                    <Typography color="text.secondary">Nema pending zahteva za odobravanje.</Typography>
                </Paper>
            ) : (
                <Stack spacing={2.5}>
                    {items.map((item) => (
                        <PendingReadingCard
                            key={item.id}
                            item={item}
                            onApprove={handleApprove}
                            onReject={handleReject}
                            isProcessing={processingId === item.id}
                        />
                    ))}
                </Stack>
            )}
        </PageShell>
    );
}

export default function AdminManualReadingsPage() {
    return (
        <AdminRoute>
            <AdminManualReadingsPageContent />
        </AdminRoute>
    );
}
