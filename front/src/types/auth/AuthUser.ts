import type { UserRole } from "../../enums/user/UserRole";
export type AuthUser = {
    id: string;
    username: string;
    role: UserRole;
};