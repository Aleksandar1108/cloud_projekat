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
import ForgotPasswordPage from './pages/auth/ForgotPasswordPage'
import MainLayout from './layouts/MainLayout'
import TelemetryAnalyticsPage from './pages/telemetry/TelemetryAnalyticsPage'
import TariffModelsPage from './pages/admin/TariffModelsPage'
import AdminBillingPage from './pages/admin/AdminBillingPage'
import AdminNetworkPage from './pages/admin/AdminNetworkPage'
import AdminBillingDeliveryPage from './pages/admin/AdminBillingDeliveryPage'

function App() {

  return (
    <>
      <Routes>
        <Route element={<MainLayout />}>
          <Route path='/' element={<DashboardPage />}></Route>
          <Route path='/users' element={<UsersPage />}></Route>
          <Route path='/monthly-billing' element={<MonthlyBillingPage />}></Route>
          <Route path='/manual-readings' element={<ManualReadingsPage />}></Route>
          <Route path='/payment/success' element={<PaymentSuccessPage />}></Route>
          <Route path='/payment/cancel' element={<PaymentCancelPage />}></Route>
          <Route path='/properties' element={<PropertiesPage />}></Route>
          <Route path='/properties/:id' element={<PropertyDetailPage />}></Route>
          <Route path='/telemetry-analytics' element={<TelemetryAnalyticsPage />}></Route>
          <Route path='/admin/tariffs' element={<TariffModelsPage />}></Route>
          <Route path='/admin/billing' element={<AdminBillingPage />}></Route>
          <Route path='/admin/network' element={<AdminNetworkPage />}></Route>
          <Route path='/admin/delivery' element={<AdminBillingDeliveryPage />}></Route>
          <Route path="*" element={<Navigate to="/404" replace />} />

        </Route>
          <Route path='/login' element={<LoginPage />}></Route>
          <Route path='/404' element={<PageNotFound />}></Route>
          <Route path="/activate" element={<ActivateAccountPage />} />
          <Route path="/forgot-password" element={<ForgotPasswordPage />}></Route>
      </Routes>
    </>
  )

}

export default App
