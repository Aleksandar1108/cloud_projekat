import type { ERoles } from "../../enums/user/UserRole";
export type AuthUser = {
    id: number;
    username: string;
    role: ERoles;
};