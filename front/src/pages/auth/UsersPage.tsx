import { useForm, type SubmitHandler } from "react-hook-form";
import { Navigate } from "react-router-dom";
import { useAuth } from "../../hooks/auth/useAuthHook";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";
import RegisterForm from "../../components/auth/RegisterForm";
import UsersList from "../../components/auth/UsersList";

type RegisterFormData = {
    email: string;
    password: string;
    confirmPassword: string;
};

function UsersPage() {
    const { isAuthenticated } = useAuth();
    const {
        register,
        handleSubmit,
        watch,
        setValue,
        reset,
        formState: { errors },
    } = useForm<RegisterFormData>();

    const onSubmit: SubmitHandler<RegisterFormData> = async (data) => {
        const response = await AuthAPIService.register(data.email, data.password);

        if (response.token) {
            alert("User registered successfully!");
        } else {
            alert("Registration failed: " + (response.error || response.message));
        }

        reset();
    };

    if (isAuthenticated) {
        return <Navigate to="/" />;
    }

    return (
        <div >
            <h1>Users</h1>
            <h2>Register new user</h2>
            <RegisterForm handleSubmit={handleSubmit} onSubmit={onSubmit} register={register} errors={errors} watch={watch} setValue={setValue} />
            <h2>All users:</h2>
            <UsersList></UsersList>
        </div>
    );
}

export default UsersPage;