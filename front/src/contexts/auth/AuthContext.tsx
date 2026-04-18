import React, { createContext, useState, type ReactNode } from "react";
import { jwtDecode } from "jwt-decode";
import type { AuthContextType } from "../../types/auth/AuthContext";
import type { AuthUser } from "../../types/auth/AuthUser";
import { SaveValueByKey, ReadValueByKey, RemoveValueByKey} from "../../helpers/local_storage";
import type { JwtTokenClaims } from "../../types/auth/JwtTokenClaims";

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const decodeJWT = (token: string): JwtTokenClaims | null => {
    try {
        const decoded = jwtDecode<JwtTokenClaims>(token);

        if (decoded && decoded.id && decoded.username && decoded.role) {
            return {
                id: decoded.id,
                username: decoded.username,
                role: decoded.role,
                exp: decoded.exp
            };
        }
        return null;
    } catch (error) {
        console.error("Error decoding JWT token:", error);
        return null;
    }
};

const isTokenExpired = (token: string): boolean => {
    try {
        const decoded = jwtDecode<JwtTokenClaims>(token);
        const currentTime = Date.now() / 1000;
        return decoded.exp ? decoded.exp < currentTime : false;
    } catch {
        return true;
    }
};

const getInitialAuthState = (): { user: AuthUser | null; token: string | null } => {
    const savedToken = ReadValueByKey("jwt");

    if (!savedToken || isTokenExpired(savedToken)) {
        RemoveValueByKey("jwt");
        return { user: null, token: null };
    }

    const claims = decodeJWT(savedToken);
    if (!claims) {
        RemoveValueByKey("jwt");
        return { user: null, token: null };
    }

    return {
        token: savedToken,
        user: {
            id: claims.id,
            username: claims.username,
            role: claims.role,
        },
    };
};

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [{ user, token }, setAuth] = useState(getInitialAuthState);

    const login = (newToken: string) => {
        const claims = decodeJWT(newToken);

        if (claims && !isTokenExpired(newToken)) {
            SaveValueByKey("jwt", newToken);

            setAuth({
                token: newToken,
                user: {
                    id: claims.id,
                    username: claims.username,
                    role: claims.role,
                },
            });
        } else {
            RemoveValueByKey("jwt");
            setAuth({ token: null, user: null });
        }
    };

    const logout = () => {
        RemoveValueByKey("jwt");
        setAuth({ token: null, user: null });
    };

    const value: AuthContextType = {
        user,
        token,
        login,
        logout,
        isAuthenticated: !!user && !!token,
        isLoading: false, // više ne postoji potreba
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export default AuthContext;