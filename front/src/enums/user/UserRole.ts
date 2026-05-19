export const ERoles = {
    User: "User",
    Admin: "Admin",
    SysAdmin: "SysAdmin"
} as const;

export type ERoles = typeof ERoles[keyof typeof ERoles];