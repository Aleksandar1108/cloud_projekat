import { useState } from "react";
import { Navigate, useNavigate, useSearchParams } from "react-router-dom";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";

function ActivateAccountPage() {

    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    const token = searchParams.get("token");

    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [message, setMessage] = useState("");

    if (!token) {
    return <Navigate to="/404"/>;
    }
    const handleActivate = async () => {    

        if (password.length < 6) {
            setMessage("Password must contain at least 6 characters.");
            return;
        }

        if (password !== confirmPassword) {
            setMessage("Passwords do not match.");
            return;
        }

        setLoading(true);

        const response = await AuthAPIService.activateUser(
            token,
            password
        );

        if (response.error) {
            setSuccess(false);
            setMessage(response.error);
        }
        else {
            setSuccess(true);
            setMessage("Account successfully activated.");
        }

        setLoading(false);
    };

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
            {
                !success && (
                    <>
                        <input
                            type="password"
                            placeholder="New password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            style={{
                                width: "100%",
                                padding: "12px 14px",
                                borderRadius: "8px",
                                border: "1px solid rgba(46, 110, 182, 0.45)",
                                backgroundColor: "#fff",
                                color: "var(--gray-900)",
                                boxSizing: "border-box",
                            }}
                        />

                        <input
                            type="password"
                            placeholder="Confirm password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            style={{
                                width: "100%",
                                padding: "12px 14px",
                                borderRadius: "8px",
                                border: "1px solid rgba(46, 110, 182, 0.45)",
                                backgroundColor: "#fff",
                                color: "var(--gray-900)",
                                boxSizing: "border-box",
                            }} />

                        <button
                            onClick={handleActivate}
                            disabled={loading}
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
                            {
                                loading
                                    ? "Activating..."
                                    : "Send"
                            }
                        </button>
                    </>
                )
            }

            {
                message && (
                    <p>
                        {message}
                    </p>
                )
            }

            {
                success && (
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