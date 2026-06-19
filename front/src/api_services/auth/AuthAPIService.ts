import axios from "axios";
import type { AuthResponse } from "../../types/auth/AuthResponse";
import type { IAuthAPIService } from "./IAuthAPIService";
import { buildApiUrl } from "../ApiBase";
import type { UserDTO } from "../../models/auth/UserDTO";

export const AuthAPIService: IAuthAPIService = {
    async login(email: string, password: string): Promise<AuthResponse> {
        try {
            const res = await axios.post<AuthResponse>(
                buildApiUrl("users/login"),
                {
                    email,
                    password
                },
                {
                    headers: {
                        "Content-Type": "application/json"
                    }
                }
            );

            return res.data;
        }
        catch (error) {
            const err: AuthResponse = {};
            if (axios.isAxiosError(error) && error.response) {
                const data: AuthResponse = error.response.data;
                err.error =
                    data?.message ||
                    data?.error ||
                    "Unknown error";
            } else {
                err.error = "Server error";
            }
            return err;
        }
    },

    async register(email: string, password: string): Promise<AuthResponse> {
        try {
            const res = await axios.post<AuthResponse>(
                buildApiUrl("users/register"),
                { email, password }
            );

            return res.data;
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data: AuthResponse = error.response.data;

                return {
                    error: data.message ||
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    },

    async createUser(email: string, role: string): Promise<AuthResponse> {
        try {
            const res = await axios.post<AuthResponse>(
                buildApiUrl("users"),
                { email, role }
            );

            return res.data;
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data: AuthResponse = error.response.data;

                return {
                    error: data.message || data.error || "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    },

    async setUserSuspension(userId: string, suspend: boolean): Promise<AuthResponse> {
        try {
            return await axios.patch<AuthResponse>(
                buildApiUrl(`users/${userId}/suspension`),
                { suspend }
            ).then(res => res.data);
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data = error.response.data;

                return {
                    error: data.message || data.type || "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    },

    async activateAccount(token: string): Promise<AuthResponse> {
        try {
            return await axios.post<AuthResponse>(
                buildApiUrl("users/activate"),
                { token }
            ).then(res => res.data);
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data: AuthResponse = error.response.data;

                return {
                    error: data.message ||
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    },
    async setPassword(token: string, password: string): Promise<AuthResponse> {
        try {
            return await axios.post<AuthResponse>(
                buildApiUrl("users/set-password"),
                { token, password }
            ).then(res => res.data);
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data: AuthResponse = error.response.data;

                return {
                    error: data.message ||
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    },
    async forgotPassword(email: string): Promise<AuthResponse> {
        try {
            return await axios.post<AuthResponse>(
                buildApiUrl("users/forgot-password"),
                { email }
            ).then(res => res.data);
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data = error.response.data;

                return {
                    error: data.message ||
                        data.type ||
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }

    },

    async changeUserRole(userId: string, newRole: string): Promise<AuthResponse> {
        try {
            return await axios.patch<AuthResponse>(
                buildApiUrl(`users/${userId}/role`),
                { newRole }
            ).then(res => res.data);
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data = error.response.data;

                return {
                    error: data.message ||
                        data.type ||
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    },

    async deleteUser(userId: string): Promise<AuthResponse> {
        try {
            return await axios.delete<AuthResponse>(
                buildApiUrl(`users/${userId}`)
            ).then(res => res.data);
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data = error.response.data;

                return {
                    error: data.message ||
                        data.type ||
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    },
    async getUsers(): Promise<UserDTO[]> {

        try {

            const res = await axios.get<UserDTO[]>(buildApiUrl("users"));

            return res.data;
        }
        catch (error) {

            if (axios.isAxiosError(error) && error.response) {

                console.error(error.response.data);
            }
            else {

                console.error("Server error");
            }

            return [];
        }
    }
};