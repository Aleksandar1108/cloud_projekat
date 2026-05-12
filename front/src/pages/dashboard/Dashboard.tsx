import LoginButton from "../../components/auth/LoginButton";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { useNavigate } from "react-router-dom";

function DashboardPage(){
    const navigate = useNavigate();
    const { isAuthenticated, user } = useAuth();

    return (
        <main>
            <h1>Dashboard Page</h1>
            <LoginButton />
            {isAuthenticated && (
                <section style={{ marginTop: "24px", maxWidth: "920px" }}>
                    <h2>Automatizovani mesecni obracun</h2>
                    <p>Ulogovani korisnik: {user?.username}</p>
                    <div
                        style={{
                            display: "flex",
                            flexWrap: "wrap",
                            gap: "8px",
                            alignItems: "center",
                            marginTop: "12px",
                        }}
                    >
                        <button
                            type="button"
                            onClick={() => navigate("/monthly-billing")}
                            style={{
                                backgroundColor: "var(--secondary)",
                                color: "var(--white)",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold",
                            }}
                        >
                            Otvori obracun
                        </button>
                        <button
                            type="button"
                            onClick={() => navigate("/manual-readings")}
                            style={{
                                backgroundColor: "#0f766e",
                                color: "var(--white)",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold",
                            }}
                        >
                            Rucni unos potrosnje
                        </button>
                        <button
                            type="button"
                            onClick={() => navigate("/hitna-upozorenja")}
                            style={{
                                backgroundColor: "var(--secondary)",
                                color: "var(--white)",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold",
                            }}
                        >
                            Obaveštenja i limit potrošnje
                        </button>
                    </div>
                </section>
            )}
        </main>
    )
}

export default DashboardPage;