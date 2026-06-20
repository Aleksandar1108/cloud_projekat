import { Button } from "@mui/material";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks/auth/useAuthHook";

type LoginButtonProps = {
    variant?: "default" | "compact" | "sidebar";
};

function LoginButton({ variant = "default" }: LoginButtonProps) {
    const navigate = useNavigate();
    const { isAuthenticated, logout } = useAuth();

    const isSidebar = variant === "sidebar";
    const isCompact = variant === "compact";

    if (!isAuthenticated) {
        return (
            <Button
                variant="contained"
                fullWidth={isSidebar}
                size={isCompact ? "small" : "medium"}
                startIcon={<LoginRoundedIcon />}
                onClick={() => navigate("/login")}
                sx={{
                    ...(isSidebar
                        ? {
                              bgcolor: "rgba(255,255,255,0.1)",
                              color: "#fff",
                              border: "1px solid rgba(255,255,255,0.12)",
                              "&:hover": { bgcolor: "rgba(255,255,255,0.16)" },
                          }
                        : {
                              background: "linear-gradient(135deg, #0ea5e9, #6366f1)",
                          }),
                }}
            >
                Prijava
            </Button>
        );
    }

    return (
        <Button
            variant={isSidebar ? "outlined" : "contained"}
            fullWidth={isSidebar}
            size={isCompact ? "small" : "medium"}
            startIcon={<LogoutRoundedIcon />}
            onClick={logout}
            sx={{
                ...(isSidebar
                    ? {
                          color: "#fecaca",
                          borderColor: "rgba(239, 68, 68, 0.35)",
                          "&:hover": {
                              borderColor: "rgba(239, 68, 68, 0.55)",
                              bgcolor: "rgba(239, 68, 68, 0.08)",
                          },
                      }
                    : {
                          bgcolor: "#ef4444",
                          "&:hover": { bgcolor: "#dc2626" },
                      }),
            }}
        >
            Odjava
        </Button>
    );
}

export default LoginButton;
