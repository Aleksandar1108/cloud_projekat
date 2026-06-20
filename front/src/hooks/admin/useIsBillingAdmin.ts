import { useMemo } from "react";
import { useAuth } from "../auth/useAuthHook";

export function useIsBillingAdmin(): boolean {
    const { user } = useAuth();

    return useMemo(() => {
        const role = String(user?.role ?? "").toLowerCase();
        return role === "admin" || role === "sysadmin";
    }, [user?.role]);
}
