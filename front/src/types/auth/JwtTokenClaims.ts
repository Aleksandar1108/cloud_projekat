import type { ERoles } from "../../enums/user/UserRole";
export type JwtTokenClaims = {
    id: number;
    username: string;
    role: ERoles;
    exp: number;
};