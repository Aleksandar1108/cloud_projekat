import { createTheme } from "@mui/material/styles";

export const appTheme = createTheme({
    palette: {
        mode: "light",
        primary: {
            main: "#0ea5e9",
            dark: "#0284c7",
            light: "#38bdf8",
        },
        secondary: {
            main: "#6366f1",
            dark: "#4f46e5",
            light: "#818cf8",
        },
        success: { main: "#10b981" },
        warning: { main: "#f59e0b" },
        error: { main: "#ef4444" },
        background: {
            default: "#f4f7fb",
            paper: "#ffffff",
        },
        text: {
            primary: "#0f172a",
            secondary: "#64748b",
        },
    },
    shape: {
        borderRadius: 14,
    },
    typography: {
        fontFamily: '"Plus Jakarta Sans", "Segoe UI", Roboto, sans-serif',
        h1: { fontWeight: 800, letterSpacing: "-0.03em" },
        h2: { fontWeight: 700, letterSpacing: "-0.02em" },
        h3: { fontWeight: 700, letterSpacing: "-0.02em" },
        h4: { fontWeight: 700 },
        h5: { fontWeight: 600 },
        h6: { fontWeight: 600 },
        button: { textTransform: "none", fontWeight: 700 },
    },
    components: {
        MuiCssBaseline: {
            styleOverrides: {
                body: {
                    backgroundColor: "#f4f7fb",
                },
            },
        },
        MuiButton: {
            styleOverrides: {
                root: {
                    borderRadius: 12,
                    boxShadow: "none",
                    "&:hover": {
                        boxShadow: "none",
                    },
                },
            },
        },
        MuiPaper: {
            styleOverrides: {
                root: {
                    backgroundImage: "none",
                },
            },
        },
    },
});
