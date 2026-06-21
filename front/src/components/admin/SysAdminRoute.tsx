import { Navigate } from "react-router-dom";
import { Box, Paper, Typography } from "@mui/material";
import LockRoundedIcon from "@mui/icons-material/LockRounded";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { useIsSysAdmin } from "../../hooks/admin/useIsSysAdmin";

type SysAdminRouteProps = {
    children: React.ReactNode;
};

export default function SysAdminRoute({ children }: SysAdminRouteProps) {
    const { isAuthenticated } = useAuth();
    const isSysAdmin = useIsSysAdmin();

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    if (!isSysAdmin) {
        return (
            <Box
                sx={{
                    minHeight: "calc(100vh - 72px)",
                    display: "grid",
                    placeItems: "center",
                    px: 2,
                }}
            >
                <Paper
                    elevation={0}
                    sx={{
                        maxWidth: 480,
                        width: "100%",
                        p: 4,
                        textAlign: "center",
                        borderRadius: 4,
                        border: "1px solid",
                        borderColor: "divider",
                    }}
                >
                    <Box
                        sx={{
                            width: 64,
                            height: 64,
                            mx: "auto",
                            mb: 2,
                            borderRadius: 3,
                            display: "grid",
                            placeItems: "center",
                            bgcolor: "rgba(239, 68, 68, 0.1)",
                            color: "#ef4444",
                        }}
                    >
                        <LockRoundedIcon />
                    </Box>
                    <Typography variant="h5" sx={{ mb: 1 }}>
                        Pristup odbijen
                    </Typography>
                    <Typography color="text.secondary">
                        Ova stranica je dostupna samo sistem administratoru.
                    </Typography>
                </Paper>
            </Box>
        );
    }

    return <>{children}</>;
}
