import type { AuthResponse } from "../types/auth/AuthResponse";
import type { IAuthAPIService } from "./auth/IAuthAPIService";

export const AuthAPIService : IAuthAPIService = {
    async login (email: string, password: string): Promise<AuthResponse> {
        alert(`Login called with email: ${email} and password: ${password}`);
        return {};
    },
    async register (email: string, password: string): Promise<AuthResponse> {
        alert(`Register called with email: ${email} and password: ${password}`);
        return {};
    }
}