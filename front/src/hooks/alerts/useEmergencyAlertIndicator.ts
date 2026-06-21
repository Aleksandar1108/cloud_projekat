import { useEffect, useState } from "react";
import { getProperties } from "../../api_services/properties/PropertyAPIService";
import { getPropertyTelemetryAnalytics } from "../../api_services/telemetry/TelemetryAnalyticsAPIService";
import { hasCriticalAlert } from "../../helpers/alerts/emergencyAlerts";
import { useAuth } from "../auth/useAuthHook";

const POLL_INTERVAL_MS = 60_000;

export function useEmergencyAlertIndicator(): boolean {
    const { isAuthenticated } = useAuth();
    const [hasActiveAlerts, setHasActiveAlerts] = useState(false);

    useEffect(() => {
        if (!isAuthenticated) {
            setHasActiveAlerts(false);
            return;
        }

        let cancelled = false;

        async function checkAlerts() {
            try {
                const properties = await getProperties();
                let criticalCount = 0;

                for (const property of properties) {
                    const analytics = await getPropertyTelemetryAnalytics(property.id).catch(() => null);
                    if (!analytics) continue;
                    criticalCount += analytics.meters.filter(hasCriticalAlert).length;
                }

                if (!cancelled) {
                    setHasActiveAlerts(criticalCount > 0);
                }
            } catch {
                if (!cancelled) {
                    setHasActiveAlerts(false);
                }
            }
        }

        void checkAlerts();
        const intervalId = window.setInterval(() => void checkAlerts(), POLL_INTERVAL_MS);

        return () => {
            cancelled = true;
            window.clearInterval(intervalId);
        };
    }, [isAuthenticated]);

    return hasActiveAlerts;
}
