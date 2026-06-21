import axios from "axios";
import { ReadValueByKey } from "../../helpers/local_storage";
import { buildApiUrl } from "../ApiBase";
import type { UserDTO } from "../../models/auth/UserDTO";
import { ERoles } from "../../enums/user/UserRole";

function authHeaders() {
    const token = ReadValueByKey("jwt");
    return token ? { Authorization: `Bearer ${token}` } : {};
}

function extractError(error: unknown, fallback: string): Error {
    if (axios.isAxiosError(error)) {
        if (error.response?.status === 401 || error.response?.status === 403) {
            return new Error("Nemate dozvolu za upravljanje korisnicima.");
        }
        const data = error.response?.data as { message?: string } | undefined;
        return new Error(data?.message ?? fallback);
    }
    return new Error(fallback);
}

export async function getSysAdminUsers(): Promise<UserDTO[]> {
    try {
        const response = await axios.get<UserDTO[]>(
            buildApiUrl("api/admin/users"),
            { headers: authHeaders() }
        );
        return response.data;
    } catch (error) {
        throw extractError(error, "Neuspesno ucitavanje korisnika.");
    }
}

export async function createSysAdminUser(
    email: string,
    password: string,
    role: ERoles
): Promise<UserDTO> {
    try {
        const response = await axios.post<UserDTO>(
            buildApiUrl("api/admin/users"),
            { email, password, role },
            { headers: authHeaders() }
        );
        return response.data;
    } catch (error) {
        throw extractError(error, "Neuspesno kreiranje korisnika.");
    }
}

export async function suspendSysAdminUser(userId: string): Promise<void> {
    try {
        await axios.post(
            buildApiUrl(`api/admin/users/${userId}/suspend`),
            {},
            { headers: authHeaders() }
        );
    } catch (error) {
        throw extractError(error, "Neuspesna suspenzija korisnika.");
    }
}

export async function deleteSysAdminUser(userId: string): Promise<void> {
    try {
        await axios.delete(
            buildApiUrl(`api/admin/users/${userId}`),
            { headers: authHeaders() }
        );
    } catch (error) {
        throw extractError(error, "Neuspesno brisanje korisnika.");
    }
}
