import { Navigate, useNavigate } from "react-router-dom";
import { useForm, type SubmitHandler } from "react-hook-form";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { AuthAPIService } from "../../api_services/AuthAPIService";
import type { LoginUserDTO } from "../../models/auth/LoginUserDTO";
import LoginForm from "../../components/auth/LoginForm";

function LoginPage() {
    const navigate = useNavigate();
    const { isAuthenticated, login } = useAuth();
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LoginUserDTO>();

    const onSubmit: SubmitHandler<LoginUserDTO> = async (data) => {
        const response = await AuthAPIService.login(data.email, data.password);

        if (response.token) {
            login(response.token);
            navigate("/");
        } else {
            console.error("Login failed:", response.error ?? response.message);
        }
    };

    if (isAuthenticated) {
        return <Navigate to="/" />;
    }

    return (
        <div >
            <h2>Login Form</h2>
            <LoginForm handleSubmit={handleSubmit} onSubmit={onSubmit} register={register} errors={errors} />
        </div>
    );
}

export default LoginPage;