import { Box, Button, Chip, Container, Grid, Paper, Typography } from "@mui/material";
import ReceiptLongRoundedIcon from "@mui/icons-material/ReceiptLongRounded";
import EditNoteRoundedIcon from "@mui/icons-material/EditNoteRounded";
import LocationCityRoundedIcon from "@mui/icons-material/LocationCityRounded";
import InsightsRoundedIcon from "@mui/icons-material/InsightsRounded";
import TuneRoundedIcon from "@mui/icons-material/TuneRounded";
import CalculateRoundedIcon from "@mui/icons-material/CalculateRounded";
import DeviceHubRoundedIcon from "@mui/icons-material/DeviceHubRounded";
import MarkEmailReadRoundedIcon from "@mui/icons-material/MarkEmailReadRounded";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import ShieldRoundedIcon from "@mui/icons-material/ShieldRounded";
import ElectricBoltRoundedIcon from "@mui/icons-material/ElectricBoltRounded";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { useIsBillingAdmin } from "../../hooks/admin/useIsBillingAdmin";
import FeatureCard from "../../components/dashboard/FeatureCard";

const consumerFeatures = [
    {
        title: "Mesecni racuni",
        description: "Pregled generisanih racuna, detalja potrosnje i online placanja.",
        path: "/monthly-billing",
        icon: <ReceiptLongRoundedIcon />,
        accent: "#0284c7",
        accentSoft: "rgba(14, 165, 233, 0.12)",
    },
    {
        title: "Rucni unos potrosnje",
        description: "Prijava stanja brojila sa prilozenom fotografijom displeja.",
        path: "/manual-readings",
        icon: <EditNoteRoundedIcon />,
        accent: "#0f766e",
        accentSoft: "rgba(15, 118, 110, 0.12)",
    },
    {
        title: "Moji objekti",
        description: "Upravljanje objektima, brojilima i uparivanje IoT uredjaja.",
        path: "/properties",
        icon: <LocationCityRoundedIcon />,
        accent: "#7c3aed",
        accentSoft: "rgba(124, 58, 237, 0.12)",
    },
    {
        title: "Telemetrija i analitika",
        description: "Grafici VT/NT potrosnje, napona i statusa u realnom vremenu.",
        path: "/telemetry-analytics",
        icon: <InsightsRoundedIcon />,
        accent: "#1d4ed8",
        accentSoft: "rgba(29, 78, 216, 0.12)",
    },
];

const adminFeatures = [
    {
        title: "Tarifni modeli",
        description: "VT/NT cene i pragovi za zelenu, plavu i crvenu zonu.",
        path: "/admin/tariffs",
        icon: <TuneRoundedIcon />,
        accent: "#16a34a",
        accentSoft: "rgba(22, 163, 74, 0.12)",
    },
    {
        title: "Mesecni obracun",
        description: "Automatizovano generisanje racuna na osnovu prikupljenih podataka.",
        path: "/admin/billing",
        icon: <CalculateRoundedIcon />,
        accent: "#0f766e",
        accentSoft: "rgba(15, 118, 110, 0.12)",
    },
    {
        title: "Status mreze i uplate",
        description: "Online/offline brojila i evidencija realizovanih uplata.",
        path: "/admin/network",
        icon: <DeviceHubRoundedIcon />,
        accent: "#b45309",
        accentSoft: "rgba(180, 83, 9, 0.12)",
    },
    {
        title: "Slanje racuna emailom",
        description: "Nadzor uspesno generisanih i poslatih racuna potrosacima.",
        path: "/admin/delivery",
        icon: <MarkEmailReadRoundedIcon />,
        accent: "#9333ea",
        accentSoft: "rgba(147, 51, 234, 0.12)",
    },
];

function DashboardPage() {
    const navigate = useNavigate();
    const { isAuthenticated, user } = useAuth();
    const isBillingAdmin = useIsBillingAdmin();

    if (!isAuthenticated) {
        return (
            <Box
                sx={{
                    minHeight: "calc(100vh - 72px)",
                    display: "grid",
                    placeItems: "center",
                    px: 2,
                    background:
                        "radial-gradient(circle at top, rgba(14, 165, 233, 0.12), transparent 35%), radial-gradient(circle at bottom, rgba(99, 102, 241, 0.12), transparent 30%)",
                }}
            >
                <Paper
                    elevation={0}
                    sx={{
                        maxWidth: 560,
                        width: "100%",
                        p: { xs: 3, md: 5 },
                        textAlign: "center",
                        borderRadius: 4,
                        border: "1px solid rgba(148, 163, 184, 0.25)",
                        background: "linear-gradient(145deg, #ffffff 0%, #f8fafc 100%)",
                    }}
                >
                    <Box
                        sx={{
                            width: 72,
                            height: 72,
                            mx: "auto",
                            mb: 2.5,
                            borderRadius: 3,
                            display: "grid",
                            placeItems: "center",
                            background: "linear-gradient(135deg, #0ea5e9, #6366f1)",
                            boxShadow: "0 16px 32px rgba(14, 165, 233, 0.28)",
                        }}
                    >
                        <ElectricBoltRoundedIcon sx={{ color: "#fff", fontSize: 36 }} />
                    </Box>
                    <Typography variant="h3" sx={{ fontSize: { xs: "1.8rem", md: "2.2rem" }, mb: 1.5 }}>
                        Smart Grid Platform
                    </Typography>
                    <Typography color="text.secondary" sx={{ mb: 3, lineHeight: 1.7 }}>
                        Centralizovana cloud platforma za pametna brojila, telemetriju, obracun i naplatu energije.
                    </Typography>
                    <Button
                        variant="contained"
                        size="large"
                        startIcon={<LoginRoundedIcon />}
                        onClick={() => navigate("/login")}
                        sx={{
                            px: 3,
                            py: 1.2,
                            background: "linear-gradient(135deg, #0ea5e9, #6366f1)",
                        }}
                    >
                        Prijavi se
                    </Button>
                </Paper>
            </Box>
        );
    }

    return (
        <Box
            sx={{
                minHeight: "calc(100vh - 72px)",
                py: { xs: 3, md: 4 },
                background:
                    "radial-gradient(circle at top right, rgba(14, 165, 233, 0.08), transparent 28%), radial-gradient(circle at bottom left, rgba(99, 102, 241, 0.08), transparent 24%)",
            }}
        >
            <Container maxWidth="xl">
                <Paper
                    elevation={0}
                    sx={{
                        p: { xs: 2.5, md: 3.5 },
                        mb: 3,
                        borderRadius: 4,
                        overflow: "hidden",
                        position: "relative",
                        border: "1px solid rgba(148, 163, 184, 0.22)",
                        background: "linear-gradient(135deg, #0f172a 0%, #1e293b 52%, #0f172a 100%)",
                        color: "#fff",
                    }}
                >
                    <Box
                        sx={{
                            position: "absolute",
                            inset: 0,
                            background:
                                "radial-gradient(circle at 85% 20%, rgba(14, 165, 233, 0.25), transparent 25%), radial-gradient(circle at 10% 80%, rgba(99, 102, 241, 0.22), transparent 28%)",
                            pointerEvents: "none",
                        }}
                    />
                    <Box sx={{ position: "relative", zIndex: 1 }}>
                        <Chip
                            label="Dobrodosli nazad"
                            sx={{
                                mb: 1.5,
                                bgcolor: "rgba(255,255,255,0.1)",
                                color: "#e2e8f0",
                                fontWeight: 700,
                            }}
                        />
                        <Typography variant="h3" sx={{ fontSize: { xs: "1.8rem", md: "2.5rem" }, mb: 1 }}>
                            Zdravo, {user?.username}
                        </Typography>
                        <Typography sx={{ color: "rgba(226, 232, 240, 0.82)", maxWidth: 680, lineHeight: 1.7 }}>
                            Upravljaj potrosnjom, prati telemetriju u realnom vremenu
                            {isBillingAdmin ? " i nadgledaj mrezu, tarife i naplatu." : "."}
                        </Typography>
                        <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap", mt: 2 }}>
                            <Chip
                                icon={<ShieldRoundedIcon sx={{ color: "#93c5fd !important" }} />}
                                label={`Uloga: ${user?.role}`}
                                sx={{ bgcolor: "rgba(14, 165, 233, 0.16)", color: "#dbeafe", fontWeight: 700 }}
                            />
                            {isBillingAdmin && (
                                <Chip
                                    label="Admin pristup aktivan"
                                    sx={{ bgcolor: "rgba(16, 185, 129, 0.16)", color: "#bbf7d0", fontWeight: 700 }}
                                />
                            )}
                        </Box>
                    </Box>
                </Paper>

                <Box sx={{ mb: 2.5 }}>
                    <Typography variant="h5" sx={{ mb: 0.5 }}>
                        Potrosac
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Brzi pristup svim funkcionalnostima za pracenje potrosnje i placanje racuna.
                    </Typography>
                </Box>

                <Grid container spacing={2.5} sx={{ mb: 4 }}>
                    {consumerFeatures.map((feature) => (
                        <Grid key={feature.path} size={{ xs: 12, sm: 6, xl: 3 }}>
                            <FeatureCard
                                title={feature.title}
                                description={feature.description}
                                icon={feature.icon}
                                accent={feature.accent}
                                accentSoft={feature.accentSoft}
                                onClick={() => navigate(feature.path)}
                            />
                        </Grid>
                    ))}
                </Grid>

                {isBillingAdmin && (
                    <>
                        <Box sx={{ mb: 2.5 }}>
                            <Typography variant="h5" sx={{ mb: 0.5 }}>
                                Administrator naplate i mreze
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                Upravljanje tarifama, mesecnim obracunom, statusom mreze i slanjem racuna.
                            </Typography>
                        </Box>

                        <Grid container spacing={2.5}>
                            {adminFeatures.map((feature) => (
                                <Grid key={feature.path} size={{ xs: 12, sm: 6, xl: 3 }}>
                                    <FeatureCard
                                        title={feature.title}
                                        description={feature.description}
                                        icon={feature.icon}
                                        accent={feature.accent}
                                        accentSoft={feature.accentSoft}
                                        onClick={() => navigate(feature.path)}
                                    />
                                </Grid>
                            ))}
                        </Grid>
                    </>
                )}
            </Container>
        </Box>
    );
}

export default DashboardPage;
