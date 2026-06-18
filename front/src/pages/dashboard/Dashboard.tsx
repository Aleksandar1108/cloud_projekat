import LoginButton from "../../components/auth/LoginButton";
import DrawerAppBar from "../../components/nav/DrawerAppBar";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { useNavigate } from "react-router-dom";

function DashboardPage(){
    const navigate = useNavigate();
    const { isAuthenticated, user } = useAuth();

    return (
        <main>
            <DrawerAppBar />
            <h1>Dashboard Page</h1>
            <LoginButton />
            {isAuthenticated && (
                <section style={{ marginTop: "24px", maxWidth: "920px" }}>
                    <h2>Automatizovani mesecni obracun</h2>
                    <p>Ulogovani korisnik: {user?.username}</p>
                    <div style={{ display: "inline-grid", gridTemplateColumns: "repeat(3, auto)", gap: "8px", alignItems: "stretch" }}>
                        <button
                            onClick={() => navigate("/monthly-billing")}
                            style={{
                                backgroundColor: "var(--secondary)",
                                color: "var(--white)",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold"
                            }}
                        >
                            Otvori obracun
                        </button>
                        <button
                            onClick={() => navigate("/manual-readings")}
                            style={{
                                backgroundColor: "#0f766e",
                                color: "var(--white)",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold"
                            }}
                        >
                            Rucni unos potrosnje
                        </button>
                        <button
                            onClick={() => navigate("/properties")}
                            style={{
                                backgroundColor: "#7c3aed",
                                color: "var(--white)",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold"
                            }}
                        >
                            Moji objekti
                        </button>
                        <button
                            onClick={() => navigate("/telemetry-analytics")}
                            style={{
                                gridColumn: "1 / 4",
                                width: "100%",
                                backgroundColor: "#1d4ed8",
                                color: "var(--white)",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold"
                            }}
                        >
                            Telemetrija i analitika
                        </button>
                    </div>
                </section>
            )}
        </main>
    )
}

export default DashboardPage;