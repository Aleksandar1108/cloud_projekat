import { Box, Button, Container, Typography } from "@mui/material";
import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import type { ReactNode } from "react";
import { useNavigate } from "react-router-dom";

type PageShellProps = {
    title: string;
    subtitle?: string;
    children: ReactNode;
    maxWidth?: "sm" | "md" | "lg" | "xl";
    showBack?: boolean;
    banner?: ReactNode;
};

export default function PageShell({
    title,
    subtitle,
    children,
    maxWidth = "lg",
    showBack = true,
    banner,
}: PageShellProps) {
    const navigate = useNavigate();

    return (
        <Box
            sx={{
                minHeight: "calc(100vh - 72px)",
                py: { xs: 3, md: 4 },
                background:
                    "radial-gradient(circle at top right, rgba(14, 165, 233, 0.08), transparent 28%), radial-gradient(circle at bottom left, rgba(99, 102, 241, 0.08), transparent 24%)",
            }}
        >
            <Container maxWidth={maxWidth}>
                {showBack && (
                    <Button
                        startIcon={<ArrowBackRoundedIcon />}
                        onClick={() => navigate("/")}
                        sx={{
                            mb: 2,
                            color: "text.secondary",
                            "&:hover": { bgcolor: "rgba(255,255,255,0.7)" },
                        }}
                    >
                        Dashboard
                    </Button>
                )}

                <Box sx={{ mb: 3 }}>
                    <Typography
                        variant="h3"
                        sx={{
                            fontSize: { xs: "1.85rem", md: "2.35rem" },
                            mb: subtitle ? 1 : 0,
                        }}
                    >
                        {title}
                    </Typography>
                    {subtitle && (
                        <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 760, lineHeight: 1.7 }}>
                            {subtitle}
                        </Typography>
                    )}
                </Box>

                {banner}

                {children}
            </Container>
        </Box>
    );
}
