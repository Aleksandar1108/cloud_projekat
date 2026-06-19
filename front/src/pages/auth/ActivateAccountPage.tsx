import { useEffect, useRef, useState } from "react";
import { Navigate, useNavigate, useSearchParams } from "react-router-dom";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";

function ActivateAccountPage() {

    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    const token = searchParams.get("token");

    const [loading, setLoading] = useState(true);
    const [message, setMessage] = useState("Activating your account...");

    const startedRef = useRef(false);

    useEffect(() => {
        if (!token || startedRef.current) {
            return;
        }
        startedRef.current = true;

        (async () => {
            const response = await AuthAPIService.activateAccount(token);

            if (response.error) {
                setMessage(response.error);
            }
            else {
                setMessage("Account successfully activated.");
                setTimeout(() => navigate("/login"), 2000);
            }

            setLoading(false);
        })();
    }, [token, navigate]);

    if (!token) {
        return <Navigate to="/404" />;
    }

    return (
        <div style={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            height: "100vh",
            gap: "16px"
        }}>
            <h1>Smart Grid App</h1>

            <p>{message}</p>

            {
                !loading && (
                    <button
                        onClick={() => navigate("/login")}
                        style={{
                            backgroundColor: "var(--secondary)",
                            color: "white",
                            border: "none",
                            padding: "10px 16px",
                            borderRadius: "6px",
                            cursor: "pointer",
                            fontWeight: "bold"
                        }}
                    >
                        Go to Login
                    </button>
                )
            }
        </div>
    );
}

export default ActivateAccountPage;
