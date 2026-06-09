import { useEffect } from 'react'
import { Navigate, Route, Routes } from 'react-router-dom'

import { authApi } from './api/authApi'
import { tokenManager } from './utils/tokenManager'

import SignInPage from './pages/SignIn/SignInPage'
import RegisterPage from './pages/Register/RegisterPage'

import HomePage from './pages/Home/HomePage'
import AccountPage from './pages/Account/AccountPage'
import ProfilePage from './pages/Account/ProfilePage'
import UserActivityReportPage from './pages/Reports/UserActivityReport/UserActivityReportPage'
import TwoFactorAuthenticationPage from './pages/Settings/TwoFactorAuthentication/TwoFactorAuthenticationPage'
import EmailWhitelistPage from './pages/Settings/EmailWhitelist/EmailWhitelistPage'
import VerifyMfaPage from './pages/VerifyMfa/VerifyMfaPage'
import ProtectedRoute from './routes/ProtectedRoute'
import AdminRoute from './routes/AdminRoute'

import HomeLayout from './layout/HomeLayout'
import SendNotificationPage from './pages/Notifications/SendNotificationPage'
import UnauthorizedPage from './pages/Unauthorized/UnauthorizedPage'

function App() {
  // useEffect(() => {
  //   const bootstrap = async () => {
  //     try {
  //       const response = await authApi.refresh()

  //       tokenManager.setToken(
  //         response.data.accessToken,
  //       )
  //     } catch (error) {
  //       console.log(
  //         'No active session found',
  //       )
  //     }
  //   }

  //   bootstrap()
  // }, [])

  return (
    <Routes>
      <Route
        path="/"
        element={<Navigate to="/home" replace />}
      />

      <Route
        path="/signin"
        element={<SignInPage />}
      />

      <Route
        path="/register"
        element={<RegisterPage />}
      />

      <Route
        path="/verify-mfa"
        element={<VerifyMfaPage />}
      />

      {/* Home layout routes */}
      <Route
        element={
          <ProtectedRoute>
            <HomeLayout />
          </ProtectedRoute>
        }
      >
        <Route
          path="/home"
          element={<HomePage />}
        />

        <Route
          path="/account"
          element={
            <AdminRoute>
              <AccountPage />
            </AdminRoute>
          }
        />

        <Route
          path="/reports/user-activity"
          element={
            <AdminRoute>
              <UserActivityReportPage />
            </AdminRoute>
          }
        />

        <Route
          path="/settings/two-factor-authentication"
          element={<TwoFactorAuthenticationPage />}
        />

        <Route
          path="/settings/email-whitelist"
          element={
            <AdminRoute>
              <EmailWhitelistPage />
            </AdminRoute>
          }
        />

        <Route
          path="/profile"
          element={<ProfilePage />}
        />

        <Route
          path="/admin/send-notification"
          element={
            <AdminRoute>
              <SendNotificationPage />
            </AdminRoute>
          }
        />

        <Route
          path="/unauthorized"
          element={<UnauthorizedPage />}
        />
      </Route>

      <Route
        path="*"
        element={<Navigate to="/home" replace />}
      />
    </Routes>
  )
}

export default App