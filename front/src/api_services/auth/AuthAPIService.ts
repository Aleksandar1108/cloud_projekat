import axios from "axios";
import type { AuthResponse } from "../../types/auth/AuthResponse";
import type { IAuthAPIService } from "./IAuthAPIService";

const API_URL = import.meta.env.VITE_SERVER;

export const AuthAPIService: IAuthAPIService = {

    async login(email: string, password: string): Promise<AuthResponse> {
        try {
            const res = await axios.post<AuthResponse>(
                `${API_URL}users/login`,
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
                err.error = error.response.data.error || "Unknown error";
            } else {
                err.error = "Server error";
            }
            return err;
        }
    },

    async register(email: string, password: string): Promise<AuthResponse> {
        try {
            const res = await axios.post<AuthResponse>(
                `${API_URL}users/register`,
                { email, password }
            );

            return res.data;
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data = error.response.data;

                return {
                    error:
                        data.message ||
                        data.type ||
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    }
}