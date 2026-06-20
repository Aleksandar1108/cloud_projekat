import { Alert } from "@mui/material";

type MockDataBannerProps = {
    message?: string;
};

export default function MockDataBanner({
    message = "Test podaci — promene se cuvaju lokalno u browseru, backend se ne koristi.",
}: MockDataBannerProps) {
    return (
        <Alert
            severity="info"
            sx={{
                mb: 3,
                borderRadius: 2.5,
                border: "1px solid rgba(14, 165, 233, 0.18)",
                bgcolor: "rgba(14, 165, 233, 0.08)",
            }}
        >
            {message}
        </Alert>
    );
}
