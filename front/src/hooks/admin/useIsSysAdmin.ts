import { useMemo } from "react";
import { useAuth } from "../auth/useAuthHook";

export function useIsSysAdmin(): boolean {
    const { user } = useAuth();

    return useMemo(() => {
        return String(user?.role ?? "").toLowerCase() === "sysadmin";
    }, [user?.role]);
}
