import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/auth/useAuthHook';

function LoginButton() {
    const navigate = useNavigate();
    const { isAuthenticated, logout } = useAuth();

    if (!isAuthenticated) {
        return (
            <button style={{ backgroundColor: 'var(--secondary)', color: 'var(--white)', border: 'none', padding: '10px 20px', borderRadius: '5px', cursor: 'pointer', fontSize: '16px', fontWeight: 'bold' }} onClick={() => navigate('/login')}>
                Login
            </button>
        )
    }
    else {
        return (
            <button style={{ backgroundColor: 'var(--secondary)', color: 'var(--white)', border: 'none', padding: '10px 20px', borderRadius: '5px', cursor: 'pointer', fontSize: '16px', fontWeight: 'bold' }} onClick={logout}>
                Logout
            </button>
        )
    }

}

export default LoginButton;