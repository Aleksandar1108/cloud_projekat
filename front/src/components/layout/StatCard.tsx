import { Paper, Typography } from "@mui/material";
import type { ReactNode } from "react";

type StatCardProps = {
    label: string;
    value: ReactNode;
    accent?: string;
};

export default function StatCard({ label, value, accent }: StatCardProps) {
    return (
        <Paper
            elevation={0}
            sx={{
                p: 2.25,
                border: "1px solid",
                borderColor: "divider",
                borderRadius: 3,
                height: "100%",
                background: "linear-gradient(145deg, #ffffff 0%, #f8fafc 100%)",
            }}
        >
            <Typography variant="body2" color="text.secondary" sx={{ mb: 0.75 }}>
                {label}
            </Typography>
            <Typography variant="h5" sx={{ fontWeight: 800, color: accent ?? "text.primary" }}>
                {value}
            </Typography>
        </Paper>
    );
}
