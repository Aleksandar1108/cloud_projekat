import type { UserRole } from "../../enums/user/UserRole";
export type JwtTokenClaims = {
    id: number;
    username: string;
    role: UserRole;
    exp: number;
};