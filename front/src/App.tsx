import { Navigate, Route, Routes } from 'react-router-dom'
import './App.css'
import LoginPage from './pages/auth/LoginPage'
import RegisterPage from './pages/auth/RegisterPage'
import PageNotFound from './pages/not_found/notFoundPage'
import DashboardPage from './pages/dashboard/Dashboard'

function App() {

  return (
  <>
    <Routes>
      <Route path='/' element={<DashboardPage />}></Route>
      <Route path='/login' element = {<LoginPage />}></Route>
      <Route path='/register' element = {<RegisterPage />}></Route>
      <Route path='/404' element = {<PageNotFound />}></Route>
      <Route path="*" element={<Navigate to="/404" replace />} />
    </Routes>
  </>
  )

}

export default App
