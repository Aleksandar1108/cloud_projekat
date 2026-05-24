import { Outlet } from "react-router-dom";
import DrawerAppBar from "../components/nav/DrawerAppBar";

function MainLayout() {

    return (
        <>
            <DrawerAppBar />

            <div style={{ paddingTop: "64px" }}>
                <Outlet />
            </div>
        </>
    );
}

export default MainLayout;