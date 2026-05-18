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
                const data: any = error.response.data;
                err.error =
                    data?.message ||
                    data?.error ||
                    data?.type ||
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
                const data: any = error.response.data;

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
    },

    async activateUser(token: string,password:string): Promise<AuthResponse>{
        try{
            return await axios.post<AuthResponse>(
                buildApiUrl("users/activate"),
                { token,password }
            ).then(res => res.data);
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                const data: any = error.response.data;

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