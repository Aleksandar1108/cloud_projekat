import type { UserRole } from "../../enums/user/UserRole";
export type JwtTokenClaims = {
    id: string;
    username: string;
    role: UserRole;
    exp: number;
};