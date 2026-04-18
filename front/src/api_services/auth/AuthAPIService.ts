import axios from "axios";
import type { AuthResponse } from "../../types/auth/AuthResponse";
import type { IAuthAPIService } from "./IAuthAPIService";

const API_URL = import.meta.env.VITE_SERVER;

export const AuthAPIService: IAuthAPIService = {

    async login(email: string, password: string): Promise<AuthResponse> {
        try {
            const formData = new FormData();
            formData.append("email", email);
            formData.append("password", password);

            const res = await axios.post<AuthResponse>(`${API_URL}login`, formData, {
                headers: { "Content-Type": "multipart/form-data" },
            });

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
            const formData = new FormData();
            formData.append("email", email);
            formData.append("password", password);

            const res = await axios.post<AuthResponse>(`${API_URL}register`, formData, {
                headers: { "Content-Type": "multipart/form-data" },
            });

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
    }
}