import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import type { Property, CreatePropertyDto, UpdatePropertyDto } from "../../types/property/Property";
import type { SmartMeter, AddSmartMeterDto, UpdateSmartMeterDto } from "../../types/property/SmartMeter";
import { ReadValueByKey } from "../../helpers/local_storage";

function authHeaders() {
    const token = ReadValueByKey("jwt");
    return token ? { Authorization: `Bearer ${token}` } : {};
}

// ── Properties ───────────────────────────────────────────────────────────────

export async function getProperties(): Promise<Property[]> {
    const response = await axios.get<Property[]>(
        buildApiUrl("api/properties"),
        { headers: authHeaders() }
    );
    return response.data;
}

export async function getPropertyById(id: string): Promise<Property> {
    const response = await axios.get<Property>(
        buildApiUrl(`api/properties/${id}`),
        { headers: authHeaders() }
    );
    return response.data;
}

export async function createProperty(dto: CreatePropertyDto): Promise<Property> {
    const response = await axios.post<Property>(
        buildApiUrl("api/properties"),
        dto,
        { headers: authHeaders() }
    );
    return response.data;
}

export async function updateProperty(id: string, dto: UpdatePropertyDto): Promise<void> {
    await axios.put(
        buildApiUrl(`api/properties/${id}`),
        dto,
        { headers: authHeaders() }
    );
}

export async function deleteProperty(id: string): Promise<void> {
    await axios.delete(
        buildApiUrl(`api/properties/${id}`),
        { headers: authHeaders() }
    );
}

// ── Smart Meters ─────────────────────────────────────────────────────────────

export async function getSmartMeters(propertyId: string): Promise<SmartMeter[]> {
    const response = await axios.get<SmartMeter[]>(
        buildApiUrl(`api/properties/${propertyId}/smart-meters`),
        { headers: authHeaders() }
    );
    return response.data;
}

export async function addSmartMeter(propertyId: string, dto: AddSmartMeterDto): Promise<SmartMeter> {
    const response = await axios.post<SmartMeter>(
        buildApiUrl(`api/properties/${propertyId}/smart-meters`),
        dto,
        { headers: authHeaders() }
    );
    return response.data;
}

export async function updateSmartMeter(propertyId: string, meterId: string, dto: UpdateSmartMeterDto): Promise<void> {
    await axios.put(
        buildApiUrl(`api/properties/${propertyId}/smart-meters/${meterId}`),
        dto,
        { headers: authHeaders() }
    );
}

export async function deleteSmartMeter(propertyId: string, meterId: string): Promise<void> {
    await axios.delete(
        buildApiUrl(`api/properties/${propertyId}/smart-meters/${meterId}`),
        { headers: authHeaders() }
    );
}

export async function registerSerialNumber(propertyId: string, meterId: string, serialNumber: string): Promise<void> {
    await axios.post(
        buildApiUrl(`api/properties/${propertyId}/smart-meters/${meterId}/register-serial`),
        { serialNumber },
        { headers: authHeaders() }
    );
}
