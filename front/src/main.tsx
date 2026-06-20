import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { CssBaseline, ThemeProvider } from '@mui/material'
import './index.css'
import App from './App.tsx'
import { AuthProvider } from './contexts/auth/AuthContext'
import { appTheme } from './theme/appTheme'

createRoot(document.getElementById('root')!).render(
    <BrowserRouter>
        <ThemeProvider theme={appTheme}>
            <CssBaseline />
            <AuthProvider>
                <App />
            </AuthProvider>
        </ThemeProvider>
    </BrowserRouter>
)
