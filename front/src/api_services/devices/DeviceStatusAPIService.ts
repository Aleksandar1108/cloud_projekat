import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import type { DeviceStatus } from "../../types/devices/DeviceStatus";

export async function getDeviceStatuses(): Promise<DeviceStatus[]> {
    try {
        const res = await axios.get<DeviceStatus[]>(buildApiUrl("api/DeviceStatuses"));
        return res.data ?? [];
    } catch (error) {
        if (axios.isAxiosError(error) && error.response?.data) {
            const serverMessage = error.response.data.message ?? error.response.data.error;
            throw new Error(serverMessage ?? "Neuspesno ucitavanje statusa brojila.");
        }
        throw new Error("Neuspesno ucitavanje statusa brojila.");
    }
}
