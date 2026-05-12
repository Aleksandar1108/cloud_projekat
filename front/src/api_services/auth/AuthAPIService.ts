import axios from "axios";
import type { AuthResponse } from "../../types/auth/AuthResponse";
import type { IAuthAPIService } from "./IAuthAPIService";
import { buildApiUrl } from "../ApiBase";

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
                const data = error.response.data as {
                    error?: string;
                    message?: string;
                };
                err.error =
                    data?.error ??
                    data?.message ??
                    (typeof data === "string" ? data : undefined) ??
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
                const data = error.response.data as {
                    error?: string;
                    message?: string;
                    type?: string;
                };

                return {
                    error:
                        data?.error ??
                        data?.message ??
                        data?.type ??
                        (typeof data === "string" ? data : undefined) ??
                        "Unknown error"
                };
            }

            return {
                error: "Server error"
            };
        }
    }
}