import { Stack, TextField, Typography } from "@mui/material";
import type { AuthFormProps } from "../../types/auth/AuthFormProps";

export default function LoginForm({ handleSubmit, onSubmit, register, errors }: AuthFormProps) {
    return (
        <Stack
            component="form"
            spacing={2}
            onSubmit={handleSubmit(onSubmit)}
            sx={{ width: "100%" }}
        >
            <TextField
                fullWidth
                label="Email adresa"
                type="email"
                placeholder="unesite@email.com"
                {...register("email", { required: true })}
                error={!!errors.email}
                helperText={errors.email ? "Email je obavezan" : " "}
            />

            <TextField
                fullWidth
                label="Lozinka"
                type="password"
                placeholder="Unesite lozinku"
                {...register("password", { required: true })}
                error={!!errors.password}
                helperText={errors.password ? "Lozinka je obavezna" : " "}
            />

            <Typography variant="caption" color="text.secondary" sx={{ mt: -1 }}>
                Koristite nalog koji vam je dodelio administrator sistema.
            </Typography>
        </Stack>
    );
}
