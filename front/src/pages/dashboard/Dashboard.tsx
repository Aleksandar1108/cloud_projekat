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
                            fontWeight: "bold",
                            marginLeft: "8px"
                        }}
                    >
                        Rucni unos potrosnje
                    </button>
                </section>
            )}
        </main>
    )
}

export default DashboardPage;