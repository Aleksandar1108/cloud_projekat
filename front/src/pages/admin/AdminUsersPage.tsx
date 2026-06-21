import { useEffect, useState } from "react";
import {
    Alert,
    Box,
    Button,
    CircularProgress,
    MenuItem,
    Paper,
    Stack,
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableRow,
    TextField,
    Typography,
} from "@mui/material";
import PersonAddRoundedIcon from "@mui/icons-material/PersonAddRounded";
import BlockRoundedIcon from "@mui/icons-material/BlockRounded";
import DeleteOutlineRoundedIcon from "@mui/icons-material/DeleteOutlineRounded";
import SysAdminRoute from "../../components/admin/SysAdminRoute";
import PageShell from "../../components/layout/PageShell";
import { ERoles } from "../../enums/user/UserRole";
import type { UserDTO } from "../../models/auth/UserDTO";
import {
    createSysAdminUser,
    deleteSysAdminUser,
    getSysAdminUsers,
    suspendSysAdminUser,
} from "../../api_services/admin/SysAdminAPIService";

function AdminUsersPageContent() {
    const [users, setUsers] = useState<UserDTO[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [role, setRole] = useState<ERoles>(ERoles.User);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const loadUsers = async () => {
        setIsLoading(true);
        setError(null);
        try {
            setUsers(await getSysAdminUsers());
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno ucitavanje korisnika.");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        void loadUsers();
    }, []);

    const handleCreateUser = async () => {
        if (!email.trim() || !password.trim()) {
            setError("Email i lozinka su obavezni.");
            return;
        }

        setIsSaving(true);
        setMessage(null);
        setError(null);

        try {
            const created = await createSysAdminUser(email.trim(), password, role);
            setUsers((prev) => [created, ...prev]);
            setEmail("");
            setPassword("");
            setRole(ERoles.User);
            setMessage("Korisnik je uspesno kreiran.");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno kreiranje korisnika.");
        } finally {
            setIsSaving(false);
        }
    };

    const handleSuspendUser = async (userId: string) => {
        const confirmed = window.confirm("Da li ste sigurni da zelite da suspendujete ovog korisnika?");
        if (!confirmed) {
            return;
        }

        setMessage(null);
        setError(null);

        try {
            await suspendSysAdminUser(userId);
            setUsers((prev) =>
                prev.map((user) =>
                    user.idUser === userId ? { ...user, isActivated: false } : user
                )
            );
            setMessage("Korisnik je suspendovan.");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesna suspenzija korisnika.");
        }
    };

    const handleDeleteUser = async (userId: string) => {
        const confirmed = window.confirm("Da li ste sigurni da zelite da obrisete ovog korisnika?");
        if (!confirmed) {
            return;
        }

        setMessage(null);
        setError(null);

        try {
            await deleteSysAdminUser(userId);
            setUsers((prev) => prev.filter((user) => user.idUser !== userId));
            setMessage("Korisnik je obrisan.");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Neuspesno brisanje korisnika.");
        }
    };

    return (
        <PageShell
            title="Upravljanje korisnicima"
            subtitle="Kreiranje, suspenzija i brisanje korisnickih naloga"
        >
            {message && <Alert severity="success" sx={{ mb: 2 }}>{message}</Alert>}
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

            <Paper elevation={0} sx={{ p: 3, mb: 3, borderRadius: 3, border: "1px solid", borderColor: "divider" }}>
                <Typography variant="h6" sx={{ mb: 2 }}>
                    Kreiraj korisnika
                </Typography>
                <Stack direction={{ xs: "column", md: "row" }} spacing={2} sx={{ alignItems: { md: "center" } }}>
                    <TextField
                        label="Email"
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        fullWidth
                    />
                    <TextField
                        label="Lozinka"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        fullWidth
                    />
                    <TextField
                        select
                        label="Uloga"
                        value={role}
                        onChange={(e) => setRole(e.target.value as ERoles)}
                        sx={{ minWidth: 180 }}
                    >
                        {Object.values(ERoles).map((item) => (
                            <MenuItem key={item} value={item}>
                                {item}
                            </MenuItem>
                        ))}
                    </TextField>
                    <Button
                        variant="contained"
                        startIcon={<PersonAddRoundedIcon />}
                        onClick={() => void handleCreateUser()}
                        disabled={isSaving}
                        sx={{ minWidth: 180, background: "linear-gradient(135deg, #0ea5e9, #6366f1)" }}
                    >
                        {isSaving ? "Cuvanje..." : "Kreiraj"}
                    </Button>
                </Stack>
            </Paper>

            <Paper elevation={0} sx={{ borderRadius: 3, border: "1px solid", borderColor: "divider", overflow: "hidden" }}>
                {isLoading ? (
                    <Box sx={{ display: "grid", placeItems: "center", py: 8 }}>
                        <CircularProgress />
                    </Box>
                ) : (
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Email</TableCell>
                                <TableCell>Uloga</TableCell>
                                <TableCell>Kreiran</TableCell>
                                <TableCell>Status</TableCell>
                                <TableCell align="right">Akcije</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {users.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={5} align="center">
                                        Nema korisnika.
                                    </TableCell>
                                </TableRow>
                            ) : (
                                users.map((user) => (
                                    <TableRow key={user.idUser}>
                                        <TableCell>{user.email}</TableCell>
                                        <TableCell>{user.role}</TableCell>
                                        <TableCell>{new Date(user.createdAt).toLocaleDateString()}</TableCell>
                                        <TableCell>
                                            {user.isActivated ? "Aktivan" : "Suspendovan"}
                                        </TableCell>
                                        <TableCell align="right">
                                            <Stack direction="row" spacing={1} sx={{ justifyContent: "flex-end" }}>
                                                <Button
                                                    size="small"
                                                    variant="outlined"
                                                    color="warning"
                                                    startIcon={<BlockRoundedIcon />}
                                                    disabled={!user.isActivated}
                                                    onClick={() => void handleSuspendUser(user.idUser)}
                                                >
                                                    Suspenduj
                                                </Button>
                                                <Button
                                                    size="small"
                                                    variant="outlined"
                                                    color="error"
                                                    startIcon={<DeleteOutlineRoundedIcon />}
                                                    onClick={() => void handleDeleteUser(user.idUser)}
                                                >
                                                    Obrisi
                                                </Button>
                                            </Stack>
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                )}
            </Paper>
        </PageShell>
    );
}

export default function AdminUsersPage() {
    return (
        <SysAdminRoute>
            <AdminUsersPageContent />
        </SysAdminRoute>
    );
}
