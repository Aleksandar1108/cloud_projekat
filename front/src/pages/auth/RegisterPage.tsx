import { Navigate, useNavigate } from "react-router-dom";
import { useForm, type SubmitHandler } from "react-hook-form";
import { useState } from "react";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";
import RegisterForm from "../../components/auth/RegisterForm";

type RegisterFormData = {
    email: string;
    password: string;
    confirmPassword: string;
};

function RegisterPage() {
    const navigate = useNavigate();
    const { isAuthenticated } = useAuth();
    const {
        register,
        handleSubmit,
        watch,
        setValue,
        reset,
        formState: { errors },
    } = useForm<RegisterFormData>();

    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const onSubmit: SubmitHandler<RegisterFormData> = async (data) => {
        setMessage(null);
        setError(null);

        const response = await AuthAPIService.register(data.email, data.password);

        if (response.error) {
            setError(response.error);
            return;
        }

        setMessage(
            "Account created. We sent you an activation email - please open the link to activate your account before logging in."
        );
        reset();
    };

    if (isAuthenticated) {
        return <Navigate to="/" />;
    }

    return (
        <main
            style={{
                display: "flex",
                flexDirection: "column",
                justifyContent: "center",
                alignItems: "center",
                minHeight: "100vh",
                gap: "16px",
                padding: "24px",
            }}
        >
            <h2>Register</h2>

            {message && (
                <div
                    style={{
                        maxWidth: "640px",
                        backgroundColor: "#dcfce7",
                        color: "#166534",
                        padding: "12px 16px",
                        borderRadius: "8px",
                        fontWeight: 600,
                    }}
                >
                    {message}
                </div>
            )}

            {error && (
                <div
                    style={{
                        maxWidth: "640px",
                        backgroundColor: "#fee2e2",
                        color: "#991b1b",
                        padding: "12px 16px",
                        borderRadius: "8px",
                        fontWeight: 600,
                    }}
                >
                    {error}
                </div>
            )}

            <div style={{ width: "100%", maxWidth: "640px" }}>
                <RegisterForm
                    handleSubmit={handleSubmit}
                    onSubmit={onSubmit}
                    register={register}
                    errors={errors}
                    watch={watch}
                    setValue={setValue}
                />
            </div>

            <button
                onClick={() => navigate("/login")}
                style={{
                    background: "none",
                    border: "none",
                    color: "var(--secondary)",
                    cursor: "pointer",
                    fontWeight: 600,
                    textDecoration: "underline",
                }}
            >
                Already have an account? Login
            </button>
        </main>
    );
}

export default RegisterPage;
