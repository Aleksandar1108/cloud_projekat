import type { FieldErrors, SubmitHandler, UseFormHandleSubmit, UseFormRegister } from "react-hook-form";
import type { LoginUserDTO } from "../../models/auth/LoginUserDTO";

export type AuthFormProps = {
    handleSubmit: UseFormHandleSubmit<LoginUserDTO>;
    onSubmit: SubmitHandler<LoginUserDTO>;
    register: UseFormRegister<LoginUserDTO>;
    errors: FieldErrors<LoginUserDTO>;
};