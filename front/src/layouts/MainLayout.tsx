import { Box } from "@mui/material";
import { Outlet } from "react-router-dom";
import DrawerAppBar from "../components/nav/DrawerAppBar";

function MainLayout() {
    return (
        <Box sx={{ display: "flex", minHeight: "100vh", bgcolor: "background.default" }}>
            <DrawerAppBar />
            <Box
                component="main"
                sx={{
                    flexGrow: 1,
                    minWidth: 0,
                    pt: { xs: "64px", lg: 0 },
                }}
            >
                <Outlet />
            </Box>
        </Box>
    );
}

export default MainLayout;
