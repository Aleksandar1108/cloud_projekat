import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import { ReadValueByKey } from "../../helpers/local_storage";
import type { PropertyTelemetryAnalytics } from "../../types/telemetry/TelemetryAnalytics";

function authHeaders() {
    const token = ReadValueByKey("jwt");
    return token ? { Authorization: `Bearer ${token}` } : {};
}

export async function getPropertyTelemetryAnalytics(propertyId: string): Promise<PropertyTelemetryAnalytics> {
    const response = await axios.get<PropertyTelemetryAnalytics>(
        buildApiUrl(`api/properties/${propertyId}/telemetry-analytics`),
        { headers: authHeaders() }
    );

    return response.data;
}
