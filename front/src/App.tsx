import { Navigate, Route, Routes } from 'react-router-dom'
import './App.css'
import LoginPage from './pages/auth/LoginPage'
import PageNotFound from './pages/not_found/notFoundPage'
import DashboardPage from './pages/dashboard/Dashboard'
import UsersPage from './pages/auth/UsersPage'
import MonthlyBillingPage from './pages/billing/MonthlyBillingPage'
import ManualReadingsPage from './pages/manual_readings/ManualReadingsPage'
import EmergencyAlertsPage from './pages/alerts/EmergencyAlertsPage'

function App() {

  return (
  <>
    <Routes>
      <Route path='/' element={<DashboardPage />}></Route>
      <Route path='/login' element = {<LoginPage />}></Route>
      <Route path='/users' element = {<UsersPage />}></Route>
      <Route path='/monthly-billing' element = {<MonthlyBillingPage />}></Route>
      <Route path='/hitna-upozorenja' element = {<EmergencyAlertsPage />}></Route>
      <Route path='/manual-readings' element = {<ManualReadingsPage />}></Route>
      <Route path='/404' element = {<PageNotFound />}></Route>
      <Route path="*" element={<Navigate to="/404" replace />} />
    </Routes>
  </>
  )

}

export default App
