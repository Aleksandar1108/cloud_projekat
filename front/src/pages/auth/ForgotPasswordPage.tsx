import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";
import LoginButton from "../../components/auth/LoginButton";

function ForgotPasswordPage() {

    const navigate = useNavigate();

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    const [email, setEmail] = useState("");

    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [message, setMessage] = useState("");

    const handleForgotPassword = async () => {

        if (!emailRegex.test(email)) {
            setMessage("Invalid email address.");
            return;
        }

        setLoading(true);

        const response = await AuthAPIService.forgotPassword(email);

        if (response.error) {
            setSuccess(false);
            setMessage(response.error);
        }
        else {
            setSuccess(true);
            setMessage("Password reset instructions have been sent to your email.");
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
            gap: "16px",
            width: "100%",
            maxWidth: "400px",
            margin: "0 auto"
        }}>
            <h1>Forgot Password</h1>

            {
                !success && (
                    <>
                        <input
                            type="email"
                            placeholder="Email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
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

                        <button
                            onClick={handleForgotPassword}
                            disabled={loading}
                            style={{
                                backgroundColor: "var(--secondary)",
                                color: "white",
                                border: "none",
                                padding: "10px 16px",
                                borderRadius: "6px",
                                cursor: "pointer",
                                fontWeight: "bold",
                                width: "100%"
                            }}
                        >
                            {
                                loading
                                    ? "Sending..."
                                    : "Send Reset Email"
                            }
                        </button>
                        <label>
                            <a href="/login">go back to login</a>
                        </label>
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

export default ForgotPasswordPage;