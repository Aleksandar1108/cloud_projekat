import { Box, Paper, Typography } from "@mui/material";
import ArrowForwardRoundedIcon from "@mui/icons-material/ArrowForwardRounded";
import type { ReactNode } from "react";

type FeatureCardProps = {
    title: string;
    description: string;
    icon: ReactNode;
    accent: string;
    accentSoft: string;
    onClick: () => void;
};

export default function FeatureCard({
    title,
    description,
    icon,
    accent,
    accentSoft,
    onClick,
}: FeatureCardProps) {
    return (
        <Paper
            elevation={0}
            onClick={onClick}
            sx={{
                p: 2.5,
                height: "100%",
                cursor: "pointer",
                border: "1px solid",
                borderColor: "rgba(148, 163, 184, 0.25)",
                borderRadius: 3,
                transition: "all 0.25s ease",
                background: "linear-gradient(145deg, #ffffff 0%, #f8fafc 100%)",
                "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: "0 20px 40px rgba(15, 23, 42, 0.08)",
                    borderColor: accent,
                    "& .feature-arrow": {
                        opacity: 1,
                        transform: "translateX(4px)",
                    },
                    "& .feature-icon": {
                        transform: "scale(1.05)",
                    },
                },
            }}
        >
            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", mb: 2 }}>
                <Box
                    className="feature-icon"
                    sx={{
                        width: 52,
                        height: 52,
                        borderRadius: 2.5,
                        display: "grid",
                        placeItems: "center",
                        bgcolor: accentSoft,
                        color: accent,
                        transition: "transform 0.25s ease",
                    }}
                >
                    {icon}
                </Box>
                <ArrowForwardRoundedIcon
                    className="feature-arrow"
                    sx={{
                        color: accent,
                        opacity: 0.35,
                        transition: "all 0.25s ease",
                    }}
                />
            </Box>
            <Typography variant="h6" sx={{ mb: 0.75, fontSize: "1.05rem" }}>
                {title}
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ lineHeight: 1.6 }}>
                {description}
            </Typography>
        </Paper>
    );
}
