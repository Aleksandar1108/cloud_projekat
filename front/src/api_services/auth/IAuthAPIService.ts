import type { UserDTO } from "../../models/auth/UserDTO";
import type { AuthResponse } from "../../types/auth/AuthResponse";

export interface IAuthAPIService {
    login(email: string, password: string): Promise<AuthResponse>;
    register(email: string, password: string,): Promise<AuthResponse>;
    createUser(email: string, role: string): Promise<AuthResponse>;
    activateAccount(token: string): Promise<AuthResponse>;
    setPassword(token: string, password: string): Promise<AuthResponse>;
    forgotPassword(email: string): Promise<AuthResponse>;
    changeUserRole(userId: string, newRole: string): Promise<AuthResponse>;
    setUserSuspension(userId: string, suspend: boolean): Promise<AuthResponse>;
    deleteUser(userId: string): Promise<AuthResponse>;
    getUsers(): Promise<UserDTO[]>;
}