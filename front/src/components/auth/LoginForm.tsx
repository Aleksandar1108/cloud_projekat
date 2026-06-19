import type { AuthFormProps } from "../../types/auth/AuthFormProps";



export default function LoginForm({ handleSubmit, onSubmit, register, errors }: AuthFormProps) {
    return (
        <form
            onSubmit={handleSubmit(onSubmit)}
            style={{
                display: "flex",
                flexDirection: "column",
                gap: "18px",
                width: "100%",
                maxWidth: "360px",
                margin: "0 auto",
                padding: "24px",
                backgroundColor: "rgba(46, 110, 182, 0.06)",
                borderRadius: "12px",
                boxShadow: "0 6px 18px rgba(46, 110, 182, 0.12)",
            }}
        >
            <div>
                <label
                    htmlFor="email"
                    style={{
                        display: "block",
                        fontSize: "14px",
                        color: "var(--secondary)",
                        fontWeight: 600,
                        marginBottom: "6px",
                    }}
                >
                    Email address
                </label>
                <input
                    id="email"
                    type="email"
                    {...register("email", { required: true })}
                    placeholder="Enter your email"
                    style={{
                        width: "100%",
                        padding: "12px 14px",
                        borderRadius: "8px",
                        border: "1px solid rgba(46, 110, 182, 0.45)",
                        backgroundColor: "#fff",
                        color: "var(--gray-900)",
                        boxSizing: "border-box",
                    }}
                />
                {errors.email && (
                    <span
                        style={{
                            color: "#dc2626",
                            fontSize: "13px",
                            marginTop: "6px",
                            display: "block",
                        }}
                    >
                        *Email* is mandatory
                    </span>
                )}
            </div>

            <div>
                <label
                    htmlFor="password"
                    style={{
                        display: "block",
                        fontSize: "14px",
                        color: "var(--secondary)",
                        fontWeight: 600,
                        marginBottom: "6px",
                    }}
                >
                    Password
                </label>
                <input
                    id="password"
                    type="password"
                    {...register("password", { required: true })}
                    placeholder="Enter your password"
                    style={{
                        width: "100%",
                        padding: "12px 14px",
                        borderRadius: "8px",
                        border: "1px solid rgba(46, 110, 182, 0.45)",
                        backgroundColor: "#fff",
                        color: "var(--gray-900)",
                        boxSizing: "border-box",
                    }}
                />
                {errors.password && (
                    <span
                        style={{
                            color: "#dc2626",
                            fontSize: "13px",
                            marginTop: "6px",
                            display: "block",
                        }}
                    >
                        *Password* is mandatory
                    </span>
                )}
            </div>

            
            <input
                type="submit"
                value="Login"
                style={{
                    backgroundColor: "var(--secondary)",
                    color: "var(--white)",
                    border: "none",
                    padding: "12px 18px",
                    borderRadius: "8px",
                    cursor: "pointer",
                    fontSize: "16px",
                    fontWeight: 700,
                }}
            />
            <label>
                <a href="/forgot-password">forgot password?</a>
            </label>
            <label>
                <a href="/register">Don't have an account? Register</a>
            </label>
        </form>
    );
}