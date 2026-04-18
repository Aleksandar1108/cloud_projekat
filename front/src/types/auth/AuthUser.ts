import type { UserRole } from "../../enums/user/UserRole";
export type AuthUser = {
    id: number;
    username: string;
    role: UserRole;
};