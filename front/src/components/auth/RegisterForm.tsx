import type { FieldErrors, UseFormHandleSubmit, UseFormRegister, SubmitHandler, UseFormWatch, UseFormSetValue } from "react-hook-form";

type RegisterFormData = {
    email: string;
    password: string;
    confirmPassword: string;
};

type RegisterFormProps = {
    handleSubmit: UseFormHandleSubmit<RegisterFormData>;
    onSubmit: SubmitHandler<RegisterFormData>;
    register: UseFormRegister<RegisterFormData>;
    errors: FieldErrors<RegisterFormData>;
    watch: UseFormWatch<RegisterFormData>;
    setValue: UseFormSetValue<RegisterFormData>;
};

export default function RegisterForm({ handleSubmit, onSubmit, register, errors, watch, setValue }: RegisterFormProps) {
    const password = watch("password");
    return (
        <form
            onSubmit={handleSubmit(onSubmit)}
            style={{
                display: "grid",
                gridTemplateColumns: "1fr 1fr 1fr auto",
                gap: "18px",
                width: "100%",
                maxWidth: "100%",
                margin: "0 auto",
                padding: "24px",
                backgroundColor: "rgba(46, 110, 182, 0.06)",
                borderRadius: "12px",
                boxShadow: "0 6px 18px rgba(46, 110, 182, 0.12)",
                alignItems: "end",
            }}
        >
            <div style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                <label
                    htmlFor="email"
                    style={{
                        display: "block",
                        fontSize: "14px",
                        color: "var(--secondary)",
                        fontWeight: 600,
                    }}
                >
                    Email address
                </label>
                <input
                    id="email"
                    type="email"
                    {...register("email", { required: true })}
                    placeholder="Enter users email"
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

            <div style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                <label
                    htmlFor="password"
                    style={{
                        display: "block",
                        fontSize: "14px",
                        color: "var(--secondary)",
                        fontWeight: 600,
                    }}
                >
                    Password
                </label>
                <input
                    id="password"
                    type="password"
                    {...register("password", { required: true })}
                    placeholder="Enter users password"
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

            <div style={{ display: "flex", flexDirection: "column", gap: "6px" }}>
                <label
                    htmlFor="repeat-password"
                    style={{
                        display: "block",
                        fontSize: "14px",
                        color: "var(--secondary)",
                        fontWeight: 600,
                    }}
                >
                    Confirm Password
                </label>
                <input
                    id="repeat-password"
                    type="password"
                    {...register("confirmPassword", {
                        required: "Confirm password is required",
                        validate: (value) => value === password || "Passwords don't match"
                    })}
                    onBlur={() => {
                        const confirmValue = watch("confirmPassword");
                        if (confirmValue && confirmValue !== password) {
                            setValue("confirmPassword", "");
                        }
                    }}
                    placeholder={errors.confirmPassword ? "Passwords don't match" : "Confirm your password"}
                    style={{
                        width: "100%",
                        padding: "12px 14px",
                        borderRadius: "8px",
                        border: errors.confirmPassword ? "1px solid #dc2626" : "1px solid rgba(46, 110, 182, 0.45)",
                        backgroundColor: "#fff",
                        color: "var(--gray-900)",
                        boxSizing: "border-box",
                    }}
                />
                {errors.confirmPassword && errors.confirmPassword.type !== "validate"}
            </div>

            <input
                type="submit"
                value="Register user"
                style={{
                    backgroundColor: "var(--secondary)",
                    color: "var(--white)",
                    border: "none",
                    padding: "14px 18px",
                    borderRadius: "8px",
                    cursor: "pointer",
                    fontSize: "16px",
                    fontWeight: 700,
                    justifySelf: "stretch",
                }}
            >
            </input>
        </form>
    );
}
        