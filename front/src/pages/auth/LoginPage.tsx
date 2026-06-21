import { Navigate, useNavigate } from "react-router-dom";
import { useForm, type SubmitHandler } from "react-hook-form";
import {
    Box,
    Button,
    Link,
    Paper,
    Stack,
    Typography,
} from "@mui/material";
import BoltRoundedIcon from "@mui/icons-material/BoltRounded";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";
import type { LoginUserDTO } from "../../models/auth/LoginUserDTO";
import LoginForm from "../../components/auth/LoginForm";

function LoginPage() {
    const navigate = useNavigate();
    const { isAuthenticated, login } = useAuth();
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LoginUserDTO>();

    const onSubmit: SubmitHandler<LoginUserDTO> = async (data) => {
        const response = await AuthAPIService.login(data.email, data.password);

        if (response.token) {
            login(response.token);
            navigate("/");
        } else {
            console.error("Login failed:", response.error ?? response.message);
        }
    };

    if (isAuthenticated) {
        return <Navigate to="/" />;
    }

    return (
        <Box
            sx={{
                minHeight: "100vh",
                display: "grid",
                placeItems: "center",
                px: 2,
                py: 4,
                background:
                    "radial-gradient(circle at top, rgba(14, 165, 233, 0.14), transparent 35%), radial-gradient(circle at bottom, rgba(99, 102, 241, 0.12), transparent 30%), #f4f7fb",
            }}
        >
            <Paper
                elevation={0}
                sx={{
                    width: "100%",
                    maxWidth: 460,
                    p: { xs: 3, md: 4 },
                    borderRadius: 4,
                    border: "1px solid rgba(148, 163, 184, 0.25)",
                    background: "linear-gradient(145deg, #ffffff 0%, #f8fafc 100%)",
                }}
            >
                <Stack spacing={2.5} sx={{ mb: 3, alignItems: "center" }}>
                    <Box
                        sx={{
                            width: 64,
                            height: 64,
                            borderRadius: 3,
                            display: "grid",
                            placeItems: "center",
                            background: "linear-gradient(135deg, #0ea5e9, #6366f1)",
                            boxShadow: "0 16px 32px rgba(14, 165, 233, 0.28)",
                        }}
                    >
                        <BoltRoundedIcon sx={{ color: "#fff", fontSize: 32 }} />
                    </Box>
                    <Box sx={{ textAlign: "center" }}>
                        <Typography variant="h4" sx={{ mb: 0.5 }}>
                            Dobrodosli nazad
                        </Typography>
                        <Typography color="text.secondary">
                            Prijavite se na Smart Grid platformu
                        </Typography>
                    </Box>
                </Stack>

                <LoginForm handleSubmit={handleSubmit} onSubmit={onSubmit} register={register} errors={errors} />

                <Stack spacing={1.5} sx={{ mt: 2.5 }}>
                    <Button
                        variant="contained"
                        fullWidth
                        size="large"
                        startIcon={<LoginRoundedIcon />}
                        onClick={handleSubmit(onSubmit)}
                        sx={{ background: "linear-gradient(135deg, #0ea5e9, #6366f1)" }}
                    >
                        Prijavi se
                    </Button>
                    <Typography variant="body2" color="text.secondary" sx={{ textAlign: "center" }}>
                        <Link component="button" underline="hover" onClick={() => navigate("/forgot-password")}>
                            Zaboravili ste lozinku?
                        </Link>
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ textAlign: "center" }}>
                        Nemate nalog?{" "}
                        <Link component="button" underline="hover" onClick={() => navigate("/users")}>
                            Registracija
                        </Link>
                    </Typography>
                </Stack>
            </Paper>
        </Box>
    );
}

export default LoginPage;
