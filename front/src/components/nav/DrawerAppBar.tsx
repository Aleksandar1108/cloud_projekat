import * as React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import {
    AppBar,
    Avatar,
    Box,
    Chip,
    Divider,
    Drawer,
    IconButton,
    List,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Toolbar,
    Typography,
    useMediaQuery,
    useTheme,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import HomeRoundedIcon from "@mui/icons-material/HomeRounded";
import ApartmentRoundedIcon from "@mui/icons-material/ApartmentRounded";
import InsightsRoundedIcon from "@mui/icons-material/InsightsRounded";
import ReceiptLongRoundedIcon from "@mui/icons-material/ReceiptLongRounded";
import TuneRoundedIcon from "@mui/icons-material/TuneRounded";
import CalculateRoundedIcon from "@mui/icons-material/CalculateRounded";
import DeviceHubRoundedIcon from "@mui/icons-material/DeviceHubRounded";
import MarkEmailReadRoundedIcon from "@mui/icons-material/MarkEmailReadRounded";
import ManageAccountsRoundedIcon from "@mui/icons-material/ManageAccountsRounded";
import PersonAddRoundedIcon from "@mui/icons-material/PersonAddRounded";
import BoltRoundedIcon from "@mui/icons-material/BoltRounded";
import LoginButton from "../auth/LoginButton";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { useIsBillingAdmin } from "../../hooks/admin/useIsBillingAdmin";
import { useIsSysAdmin } from "../../hooks/admin/useIsSysAdmin";

const drawerWidth = 280;

type NavItem = {
    label: string;
    path: string;
    icon: React.ReactNode;
    adminOnly?: boolean;
    sysAdminOnly?: boolean;
};

const baseNavItems: NavItem[] = [
    { label: "Dashboard", path: "/", icon: <HomeRoundedIcon /> },
    { label: "Objekti", path: "/properties", icon: <ApartmentRoundedIcon /> },
    { label: "Telemetrija", path: "/telemetry-analytics", icon: <InsightsRoundedIcon /> },
    { label: "Racuni", path: "/monthly-billing", icon: <ReceiptLongRoundedIcon /> },
    { label: "Register", path: "/users", icon: <PersonAddRoundedIcon /> },
];

const sysAdminNavItems: NavItem[] = [
    { label: "Korisnici", path: "/admin/users", icon: <ManageAccountsRoundedIcon />, sysAdminOnly: true },
];

const adminNavItems: NavItem[] = [
    { label: "Tarifni modeli", path: "/admin/tariffs", icon: <TuneRoundedIcon />, adminOnly: true },
    { label: "Mesecni obracun", path: "/admin/billing", icon: <CalculateRoundedIcon />, adminOnly: true },
    { label: "Status mreze", path: "/admin/network", icon: <DeviceHubRoundedIcon />, adminOnly: true },
    { label: "Email racuni", path: "/admin/delivery", icon: <MarkEmailReadRoundedIcon />, adminOnly: true },
];

function NavList({
    items,
    currentPath,
    onNavigate,
}: {
    items: NavItem[];
    currentPath: string;
    onNavigate: (path: string) => void;
}) {
    return (
        <List sx={{ px: 1.5, py: 1 }}>
            {items.map((item) => {
                const active = currentPath === item.path;
                const textColor = active ? "#e0f2fe" : "#f1f5f9";
                const iconColor = active ? "#38bdf8" : "#cbd5e1";

                return (
                    <ListItemButton
                        key={item.path}
                        onClick={() => onNavigate(item.path)}
                        sx={{
                            mb: 0.75,
                            borderRadius: 2.5,
                            py: 1.1,
                            bgcolor: active ? "rgba(14, 165, 233, 0.22)" : "transparent",
                            border: active ? "1px solid rgba(56, 189, 248, 0.35)" : "1px solid transparent",
                            color: textColor,
                            "&:hover": {
                                bgcolor: active ? "rgba(14, 165, 233, 0.28)" : "rgba(255,255,255,0.08)",
                                color: "#ffffff",
                                "& .MuiListItemIcon-root": {
                                    color: active ? "#7dd3fc" : "#ffffff",
                                },
                            },
                        }}
                    >
                        <ListItemIcon
                            sx={{
                                minWidth: 42,
                                color: iconColor,
                            }}
                        >
                            {item.icon}
                        </ListItemIcon>
                        <ListItemText
                            primary={item.label}
                            slotProps={{
                                primary: {
                                    sx: {
                                        fontWeight: active ? 700 : 600,
                                        fontSize: "0.95rem",
                                        color: textColor,
                                    },
                                },
                            }}
                        />
                    </ListItemButton>
                );
            })}
        </List>
    );
}

function SidebarContent({
    currentPath,
    onNavigate,
    navItems,
    userLabel,
    userRole,
}: {
    currentPath: string;
    onNavigate: (path: string) => void;
    navItems: NavItem[];
    userLabel?: string;
    userRole?: string;
}) {
    const consumerItems = navItems.filter((item) => !item.adminOnly && !item.sysAdminOnly);
    const adminItems = navItems.filter((item) => item.adminOnly);
    const sysAdminItems = navItems.filter((item) => item.sysAdminOnly);

    return (
        <Box
            sx={{
                height: "100vh",
                display: "flex",
                flexDirection: "column",
                overflow: "hidden",
                background: "linear-gradient(180deg, #0f172a 0%, #111827 45%, #0b1220 100%)",
                color: "#fff",
            }}
        >
            <Box sx={{ px: 2.5, pt: 3, pb: 2, flexShrink: 0 }}>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                    <Box
                        sx={{
                            width: 44,
                            height: 44,
                            borderRadius: 2.5,
                            display: "grid",
                            placeItems: "center",
                            background: "linear-gradient(135deg, #0ea5e9, #6366f1)",
                            boxShadow: "0 10px 24px rgba(14, 165, 233, 0.35)",
                        }}
                    >
                        <BoltRoundedIcon sx={{ color: "#fff" }} />
                    </Box>
                    <Box>
                        <Typography sx={{ fontWeight: 800, fontSize: "1.05rem", lineHeight: 1.1, color: "#ffffff" }}>
                            Smart Grid
                        </Typography>
                        <Typography sx={{ fontSize: "0.78rem", color: "rgba(255,255,255,0.72)" }}>
                            Smart Metering Platform
                        </Typography>
                    </Box>
                </Box>
            </Box>

            <Divider sx={{ borderColor: "rgba(255,255,255,0.12)", flexShrink: 0 }} />

            <Box
                sx={{
                    flex: 1,
                    minHeight: 0,
                    overflowY: "auto",
                    overflowX: "hidden",
                    py: 1,
                    "&::-webkit-scrollbar": {
                        width: "6px",
                    },
                    "&::-webkit-scrollbar-thumb": {
                        backgroundColor: "rgba(255,255,255,0.18)",
                        borderRadius: "999px",
                    },
                }}
            >
                <Typography
                    sx={{
                        px: 2.5,
                        pt: 1.5,
                        pb: 0.5,
                        fontSize: "0.72rem",
                        letterSpacing: "0.08em",
                        fontWeight: 700,
                        color: "rgba(255,255,255,0.58)",
                    }}
                >
                    POTROSAC
                </Typography>
                <NavList items={consumerItems} currentPath={currentPath} onNavigate={onNavigate} />

                {adminItems.length > 0 && (
                    <>
                        <Typography
                            sx={{
                                px: 2.5,
                                pt: 1.5,
                                pb: 0.5,
                                fontSize: "0.72rem",
                                letterSpacing: "0.08em",
                                fontWeight: 700,
                                color: "rgba(255,255,255,0.58)",
                            }}
                        >
                            ADMIN NAPLATE
                        </Typography>
                        <NavList items={adminItems} currentPath={currentPath} onNavigate={onNavigate} />
                    </>
                )}

                {sysAdminItems.length > 0 && (
                    <>
                        <Typography
                            sx={{
                                px: 2.5,
                                pt: 1.5,
                                pb: 0.5,
                                fontSize: "0.72rem",
                                letterSpacing: "0.08em",
                                fontWeight: 700,
                                color: "rgba(255,255,255,0.58)",
                            }}
                        >
                            SISTEM ADMIN
                        </Typography>
                        <NavList items={sysAdminItems} currentPath={currentPath} onNavigate={onNavigate} />
                    </>
                )}
            </Box>

            {userLabel && (
                <Box
                    sx={{
                        p: 2,
                        flexShrink: 0,
                        borderTop: "1px solid rgba(255,255,255,0.12)",
                        bgcolor: "rgba(15, 23, 42, 0.92)",
                        backdropFilter: "blur(8px)",
                    }}
                >
                    <Box sx={{ display: "flex", alignItems: "center", gap: 1.25, mb: 1.5 }}>
                        <Avatar
                            sx={{
                                width: 40,
                                height: 40,
                                bgcolor: "rgba(14, 165, 233, 0.18)",
                                color: "#7dd3fc",
                                fontWeight: 800,
                                fontSize: "0.95rem",
                            }}
                        >
                            {userLabel.charAt(0).toUpperCase()}
                        </Avatar>
                        <Box sx={{ minWidth: 0 }}>
                            <Typography noWrap sx={{ fontWeight: 700, fontSize: "0.92rem", color: "#f8fafc" }}>
                                {userLabel}
                            </Typography>
                            <Chip
                                label={userRole ?? "User"}
                                size="small"
                                sx={{
                                    mt: 0.35,
                                    height: 22,
                                    fontSize: "0.68rem",
                                    fontWeight: 700,
                                    bgcolor: "rgba(99, 102, 241, 0.28)",
                                    color: "#e0e7ff",
                                }}
                            />
                        </Box>
                    </Box>
                    <LoginButton variant="sidebar" />
                </Box>
            )}
        </Box>
    );
}

export default function DrawerAppBar() {
    const theme = useTheme();
    const isDesktop = useMediaQuery(theme.breakpoints.up("lg"));
    const [mobileOpen, setMobileOpen] = React.useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const { isAuthenticated, user } = useAuth();
    const isBillingAdmin = useIsBillingAdmin();
    const isSysAdmin = useIsSysAdmin();

    const navItems = React.useMemo(() => {
        const items = [...baseNavItems];

        if (isAuthenticated && isBillingAdmin) {
            items.push(...adminNavItems);
        }

        if (isAuthenticated && isSysAdmin) {
            items.push(...sysAdminNavItems);
        }

        return items;
    }, [isAuthenticated, isBillingAdmin, isSysAdmin]);

    const handleNavigate = (path: string) => {
        navigate(path);
        setMobileOpen(false);
    };

    const sidebar = (
        <SidebarContent
            currentPath={location.pathname}
            onNavigate={handleNavigate}
            navItems={navItems}
            userLabel={isAuthenticated ? user?.username : undefined}
            userRole={user?.role}
        />
    );

    return (
        <>
            {!isDesktop && (
                <AppBar
                    position="fixed"
                    elevation={0}
                    sx={{
                        bgcolor: "rgba(255,255,255,0.88)",
                        backdropFilter: "blur(12px)",
                        borderBottom: "1px solid rgba(148, 163, 184, 0.2)",
                        color: "text.primary",
                        zIndex: (t) => t.zIndex.drawer + 1,
                    }}
                >
                    <Toolbar>
                        <IconButton edge="start" onClick={() => setMobileOpen(true)} sx={{ mr: 1 }}>
                            <MenuIcon />
                        </IconButton>
                        <Typography sx={{ flexGrow: 1, fontWeight: 800 }}>Smart Grid</Typography>
                        <LoginButton variant="compact" />
                    </Toolbar>
                </AppBar>
            )}

            {isDesktop ? (
                <Drawer
                    variant="permanent"
                    sx={{
                        width: drawerWidth,
                        flexShrink: 0,
                        display: { xs: "none", lg: "block" },
                        "& .MuiDrawer-paper": {
                            width: drawerWidth,
                            boxSizing: "border-box",
                            border: "none",
                            position: "fixed",
                            top: 0,
                            left: 0,
                            height: "100vh",
                            overflow: "hidden",
                        },
                    }}
                    open
                >
                    {sidebar}
                </Drawer>
            ) : (
                <Drawer
                    variant="temporary"
                    open={mobileOpen}
                    onClose={() => setMobileOpen(false)}
                    ModalProps={{ keepMounted: true }}
                    sx={{
                        display: { lg: "none" },
                        "& .MuiDrawer-paper": {
                            width: drawerWidth,
                            boxSizing: "border-box",
                            height: "100vh",
                            overflow: "hidden",
                        },
                    }}
                >
                    {sidebar}
                </Drawer>
            )}
        </>
    );
}
