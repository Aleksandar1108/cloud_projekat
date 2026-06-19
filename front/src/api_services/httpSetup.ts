import axios from "axios";
import { ReadValueByKey, RemoveValueByKey } from "../helpers/local_storage";

let initialized = false;

/**
 * Registers global axios interceptors:
 *  - attaches the JWT bearer token to every outgoing request
 *  - on 401 responses clears the stored token and redirects to /login
 */
export function setupHttpInterceptors(): void {
    if (initialized) {
        return;
    }
    initialized = true;

    axios.interceptors.request.use((config) => {
        const token = ReadValueByKey("jwt");
        if (token) {
            config.headers = config.headers ?? {};
            if (!config.headers.Authorization) {
                config.headers.Authorization = `Bearer ${token}`;
            }
        }
        return config;
    });

    axios.interceptors.response.use(
        (response) => response,
        (error) => {
            if (axios.isAxiosError(error) && error.response?.status === 401) {
                RemoveValueByKey("jwt");
                if (window.location.pathname !== "/login") {
                    window.location.assign("/login");
                }
            }
            return Promise.reject(error);
        }
    );
}
