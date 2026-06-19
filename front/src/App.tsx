import { Navigate, Route, Routes } from 'react-router-dom'
import './App.css'
import LoginPage from './pages/auth/LoginPage'
import PageNotFound from './pages/not_found/notFoundPage'
import DashboardPage from './pages/dashboard/Dashboard'
import UsersPage from './pages/auth/UsersPage'
import MonthlyBillingPage from './pages/billing/MonthlyBillingPage'
import ManualReadingsPage from './pages/manual_readings/ManualReadingsPage'
import PaymentSuccessPage from './pages/payments/PaymentSuccessPage'
import PaymentCancelPage from './pages/payments/PaymentCancelPage'
import PropertiesPage from './pages/properties/PropertiesPage'
import PropertyDetailPage from './pages/properties/PropertyDetailPage'
import ActivateAccountPage from './pages/auth/ActivateAccountPage'
import SetPasswordPage from './pages/auth/SetPasswordPage'
import TariffModelsPage from './pages/tariffs/TariffModelsPage'
import PaymentsPage from './pages/payments/PaymentsPage'
import NetworkOverviewPage from './pages/network/NetworkOverviewPage'
import ForgotPasswordPage from './pages/auth/ForgotPasswordPage'
import RegisterPage from './pages/auth/RegisterPage'
import MainLayout from './layouts/MainLayout'
import TelemetryAnalyticsPage from './pages/telemetry/TelemetryAnalyticsPage'
import { ProtectedRoute } from './components/protcetedRoute/ProtectedRoute'
import { ERoles } from './enums/user/UserRole'

function App() {

  return (
    <>
      <Routes>
        <Route element={<MainLayout />}>
          <Route path='/' element={<ProtectedRoute><DashboardPage /></ProtectedRoute>}></Route>
          <Route path='/users' element={<ProtectedRoute requiredRole={ERoles.SysAdmin}><UsersPage /></ProtectedRoute>}></Route>
          <Route path='/monthly-billing' element={<ProtectedRoute><MonthlyBillingPage /></ProtectedRoute>}></Route>
          <Route path='/tariffs' element={<ProtectedRoute requiredRole={[ERoles.Admin, ERoles.SysAdmin]}><TariffModelsPage /></ProtectedRoute>}></Route>
          <Route path='/payments' element={<ProtectedRoute requiredRole={[ERoles.Admin, ERoles.SysAdmin]}><PaymentsPage /></ProtectedRoute>}></Route>
          <Route path='/network' element={<ProtectedRoute requiredRole={[ERoles.Admin, ERoles.SysAdmin]}><NetworkOverviewPage /></ProtectedRoute>}></Route>
          <Route path='/manual-readings' element={<ProtectedRoute><ManualReadingsPage /></ProtectedRoute>}></Route>
          <Route path='/payment/success' element={<ProtectedRoute><PaymentSuccessPage /></ProtectedRoute>}></Route>
          <Route path='/payment/cancel' element={<ProtectedRoute><PaymentCancelPage /></ProtectedRoute>}></Route>
          <Route path='/properties' element={<ProtectedRoute><PropertiesPage /></ProtectedRoute>}></Route>
          <Route path='/properties/:id' element={<ProtectedRoute><PropertyDetailPage /></ProtectedRoute>}></Route>
          <Route path='/telemetry-analytics' element={<ProtectedRoute><TelemetryAnalyticsPage /></ProtectedRoute>}></Route>
          <Route path="*" element={<Navigate to="/404" replace />} />

        </Route>
          <Route path='/login' element={<LoginPage />}></Route>
          <Route path='/register' element={<RegisterPage />}></Route>
          <Route path='/404' element={<PageNotFound />}></Route>
          <Route path="/activate" element={<ActivateAccountPage />} />
          <Route path="/set-password" element={<SetPasswordPage />} />
          <Route path="/forgot-password" element={<ForgotPasswordPage />}></Route>
      </Routes>
    </>
  )

}

export default App
