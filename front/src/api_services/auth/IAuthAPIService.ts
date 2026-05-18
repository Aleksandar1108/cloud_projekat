import type { AuthResponse } from "../../types/auth/AuthResponse";

export interface IAuthAPIService {
    login(email: string, password: string): Promise<AuthResponse>;
    register(email: string, password: string,): Promise<AuthResponse>;
    activateUser(token: string,password:string): Promise<AuthResponse>;
}