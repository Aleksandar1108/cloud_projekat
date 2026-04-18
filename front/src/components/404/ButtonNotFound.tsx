import { useNavigate } from 'react-router-dom';

function ButtonNotFound(){
    const navigate = useNavigate();

    return (
        <button style={{ backgroundColor: 'var(--secondary)', color: 'var(--white)', border: 'none', padding: '10px 20px', borderRadius: '5px', cursor: 'pointer', fontSize: '16px', fontWeight: 'bold' }} onClick={() => navigate('/')}>
            Go to Home
        </button>
    )
}

export default ButtonNotFound;