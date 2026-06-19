import { useForm, type SubmitHandler } from "react-hook-form";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";
import UsersList from "../../components/auth/UsersList";
import type { UserDTO } from "../../models/auth/UserDTO";
import { ERoles } from "../../enums/user/UserRole";
import { useEffect, useState } from "react";

type CreateUserFormData = {
    email: string;
    role: string;
};

function UsersPage() {
    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<CreateUserFormData>({
        defaultValues: { email: "", role: ERoles.User },
    });

    const [users, setUsers] = useState<UserDTO[]>([]);

    const loadUsers = async () => {
        const response = await AuthAPIService.getUsers();
        setUsers(response);
    };

    useEffect(() => {
        loadUsers();
    }, []);

    const onSubmit: SubmitHandler<CreateUserFormData> = async (data) => {
        const response = await AuthAPIService.createUser(data.email, data.role);

        if (!response.error) {
            alert("User created. An activation email has been sent.");
            await loadUsers();
            reset();
        } else {
            alert("User creation failed: " + response.error);
        }
    };

    return (
        <main>
            <h1>Users</h1>
            <h2>Create new user</h2>

            <form
                onSubmit={handleSubmit(onSubmit)}
                style={{
                    display: "grid",
                    gridTemplateColumns: "1fr 1fr auto",
                    gap: "18px",
                    width: "100%",
                    padding: "24px",
                    backgroundColor: "rgba(46, 110, 182, 0.06)",
                    borderRadius: "12px",
                    boxShadow: "0 6px 18px rgba(46, 110, 182, 0.12)",
                    alignItems: "end",
                }}
            >
                <div style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                    <label htmlFor="email" style={{ fontSize: "14px", color: "var(--secondary)", fontWeight: 600 }}>
                        Email address
                    </label>
                    <input
                        id="email"
                        type="email"
                        {...register("email", { required: true })}
                        placeholder="Enter user's email"
                        style={{
                            width: "100%",
                            padding: "12px 14px",
                            borderRadius: "8px",
                            border: "1px solid rgba(46, 110, 182, 0.45)",
                            backgroundColor: "#fff",
                            boxSizing: "border-box",
                        }}
                    />
                    {errors.email && (
                        <span style={{ color: "#dc2626", fontSize: "13px" }}>*Email* is mandatory</span>
                    )}
                </div>

                <div style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                    <label htmlFor="role" style={{ fontSize: "14px", color: "var(--secondary)", fontWeight: 600 }}>
                        Role
                    </label>
                    <select
                        id="role"
                        {...register("role", { required: true })}
                        style={{
                            width: "100%",
                            padding: "12px 14px",
                            borderRadius: "8px",
                            border: "1px solid rgba(46, 110, 182, 0.45)",
                            backgroundColor: "#fff",
                            boxSizing: "border-box",
                        }}
                    >
                        {Object.values(ERoles).map((role) => (
                            <option key={role} value={role}>
                                {role}
                            </option>
                        ))}
                    </select>
                </div>

                <input
                    type="submit"
                    value="Create user"
                    style={{
                        backgroundColor: "var(--secondary)",
                        color: "var(--white)",
                        border: "none",
                        padding: "14px 18px",
                        borderRadius: "8px",
                        cursor: "pointer",
                        fontSize: "16px",
                        fontWeight: 700,
                    }}
                />
            </form>

            <h2>All users:</h2>
            <UsersList users={users} setUsers={setUsers}></UsersList>
        </main>
    );
}

export default UsersPage;
