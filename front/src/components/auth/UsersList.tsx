import type { Dispatch, SetStateAction } from "react";
import { ERoles } from "../../enums/user/UserRole";
import type { UserDTO } from "../../models/auth/UserDTO";
import { AuthAPIService } from "../../api_services/auth/AuthAPIService";

type UsersListProps = {
    users: UserDTO[];
    setUsers: Dispatch<SetStateAction<UserDTO[]>>;
};

function UsersList({
    users,
    setUsers
}: UsersListProps) {

    const handleRoleChange = async ( userId: string, newRole: ERoles) => {
        try {

            const result = await AuthAPIService.changeUserRole(userId,newRole);

            if (result.error) {
                alert(`Failed to change user role: ${result.error}`);
            }
            else {

                setUsers(prev => prev.map(user => user.idUser === userId? { ...user, role: newRole } : user));
            }
        }
        catch {
            alert("Failed to change user role.");
        }
    };

    const handleDeleteUser = async (userId: string) => {

        const confirmed = window.confirm( "Are you sure you want to delete this user?");
        if (!confirmed)
            return;

        try {

            const result = await AuthAPIService.deleteUser(userId);

            if (result.error) {
                alert(`Failed to delete user: ${result.error}`);
            }
            else {
                setUsers(prev => prev.filter( user => user.idUser !== userId));
            }
        }
        catch {

            alert("Failed to delete user.");
        }
    };

    const handleToggleSuspension = async (userId: string, suspend: boolean) => {
        try {
            const result = await AuthAPIService.setUserSuspension(userId, suspend);

            if (result.error) {
                alert(`Failed to update suspension: ${result.error}`);
            }
            else {
                setUsers(prev => prev.map(user => user.idUser === userId ? { ...user, isSuspended: suspend } : user));
            }
        }
        catch {
            alert("Failed to update suspension.");
        }
    };

    const handleResetPassword = async ( email: string) => {
        try {
            const result =
                await AuthAPIService.forgotPassword(email);

            if (result.error) {

                alert(
                    `Failed to reset password: ${result.error}`
                );
            }
            else {
                alert(
                    "Password reset email sent successfully."
                );
            }
        }
        catch {
            alert("Failed to reset password.");
        }
    };

    return (
        <div style={{
            minHeight: "100vh",
            backgroundColor: "#f4f7fb",
            padding: "40px",
            boxSizing: "border-box"
        }}>

            <div style={{
                maxWidth: "1300px",
                margin: "0 auto"
            }}>

                <div style={{
                    backgroundColor: "#ffffff",
                    borderRadius: "14px",
                    overflow: "hidden",
                    boxShadow: "0 4px 18px rgba(0,0,0,0.08)"
                }}>

                    <table style={{
                        width: "100%",
                        borderCollapse: "collapse"
                    }}>

                        <thead style={{
                            backgroundColor: "#eef2ff"
                        }}>
                            <tr>

                                <th style={{
                                    padding: "16px",
                                    textAlign: "left"
                                }}>
                                    User ID
                                </th>

                                <th style={{
                                    padding: "16px",
                                    textAlign: "left"
                                }}>
                                    Email
                                </th>

                                <th style={{
                                    padding: "16px",
                                    textAlign: "left"
                                }}>
                                    Role
                                </th>

                                <th style={{
                                    padding: "16px",
                                    textAlign: "left"
                                }}>
                                    Created At
                                </th>

                                <th style={{
                                    padding: "16px",
                                    textAlign: "left"
                                }}>
                                    Activated
                                </th>

                                <th style={{
                                    padding: "16px",
                                    textAlign: "left"
                                }}>
                                    Suspended
                                </th>

                                <th style={{
                                    padding: "16px",
                                    textAlign: "left"
                                }}>
                                    Actions
                                </th>

                            </tr>
                        </thead>

                        <tbody>

                            {
                                users.length === 0 ? (

                                    <tr>
                                        <td
                                            colSpan={7}
                                            style={{
                                                padding: "24px",
                                                textAlign: "center"
                                            }}
                                        >
                                            No users found.
                                        </td>
                                    </tr>

                                ) : (

                                    users.map((user) => (

                                        <tr
                                            key={user.idUser}
                                            style={{
                                                borderBottom:
                                                    "1px solid #e5e7eb"
                                            }}
                                        >

                                            <td style={{
                                                padding: "16px"
                                            }}>
                                                {user.idUser}
                                            </td>

                                            <td style={{
                                                padding: "16px"
                                            }}>
                                                {user.email}
                                            </td>

                                            <td style={{
                                                padding: "16px"
                                            }}>

                                                <select
                                                    value={user.role}
                                                    onChange={(e) =>
                                                        handleRoleChange(
                                                            user.idUser,
                                                            e.target.value as ERoles
                                                        )
                                                    }
                                                    style={{
                                                        padding: "10px",
                                                        borderRadius: "8px",
                                                        border:
                                                            "1px solid #d1d5db",
                                                        cursor: "pointer"
                                                    }}
                                                >

                                                    {
                                                        Object.values(ERoles)
                                                            .map(role => (

                                                                <option
                                                                    key={role}
                                                                    value={role}
                                                                >
                                                                    {role}
                                                                </option>
                                                            ))
                                                    }

                                                </select>

                                            </td>

                                            <td style={{
                                                padding: "16px"
                                            }}>
                                                {
                                                    new Date(
                                                        user.createdAt
                                                    ).toLocaleDateString()
                                                }
                                            </td>

                                            <td style={{
                                                padding: "16px"
                                            }}>

                                                <span style={{
                                                    padding: "6px 10px",
                                                    borderRadius: "999px",
                                                    fontSize: "13px",
                                                    fontWeight: 600,
                                                    backgroundColor:
                                                        user.isActivated
                                                            ? "#dcfce7"
                                                            : "#fee2e2",
                                                    color:
                                                        user.isActivated
                                                            ? "#166534"
                                                            : "#991b1b"
                                                }}>
                                                    {
                                                        user.isActivated
                                                            ? "True"
                                                            : "False"
                                                    }
                                                </span>

                                            </td>

                                            <td style={{
                                                padding: "16px"
                                            }}>

                                                <span style={{
                                                    padding: "6px 10px",
                                                    borderRadius: "999px",
                                                    fontSize: "13px",
                                                    fontWeight: 600,
                                                    backgroundColor:
                                                        user.isSuspended
                                                            ? "#fee2e2"
                                                            : "#dcfce7",
                                                    color:
                                                        user.isSuspended
                                                            ? "#991b1b"
                                                            : "#166534"
                                                }}>
                                                    {
                                                        user.isSuspended
                                                            ? "Suspended"
                                                            : "Active"
                                                    }
                                                </span>

                                            </td>

                                            <td style={{
                                                padding: "16px",
                                                display: "flex",
                                                gap: "10px"
                                            }}>

                                                <button
                                                    onClick={() =>
                                                        handleToggleSuspension(
                                                            user.idUser,
                                                            !user.isSuspended
                                                        )
                                                    }
                                                    style={{
                                                        backgroundColor:
                                                            user.isSuspended
                                                                ? "#16a34a"
                                                                : "#d97706",
                                                        color: "#ffffff",
                                                        border: "none",
                                                        padding:
                                                            "10px 14px",
                                                        borderRadius:
                                                            "8px",
                                                        cursor: "pointer",
                                                        fontWeight: 600
                                                    }}
                                                >
                                                    {
                                                        user.isSuspended
                                                            ? "Activate"
                                                            : "Suspend"
                                                    }
                                                </button>

                                                <button
                                                    onClick={() =>
                                                        handleResetPassword(
                                                            user.email
                                                        )
                                                    }
                                                    style={{
                                                        backgroundColor:
                                                            "#2563eb",
                                                        color: "#ffffff",
                                                        border: "none",
                                                        padding:
                                                            "10px 14px",
                                                        borderRadius:
                                                            "8px",
                                                        cursor: "pointer",
                                                        fontWeight: 600
                                                    }}
                                                >
                                                    Reset Password
                                                </button>

                                                <button
                                                    onClick={() =>
                                                        handleDeleteUser(
                                                            user.idUser
                                                        )
                                                    }
                                                    style={{
                                                        backgroundColor:
                                                            "#dc2626",
                                                        color: "#ffffff",
                                                        border: "none",
                                                        padding:
                                                            "10px 14px",
                                                        borderRadius:
                                                            "8px",
                                                        cursor: "pointer",
                                                        fontWeight: 600
                                                    }}
                                                >
                                                    Delete User
                                                </button>

                                            </td>

                                        </tr>
                                    ))
                                )
                            }

                        </tbody>

                    </table>

                </div>

            </div>

        </div>
    );
}

export default UsersList;