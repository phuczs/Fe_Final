import { Navigate } from 'react-router-dom'
import { tokenManager } from '../utils/tokenManager'
import { jwtHelper } from '../utils/jwtHelper'

export const isAdminRole = (role) => {
  if (!role) return false
  const r = String(role).toLowerCase()
  return (
    r === 'admin' ||
    r === 'administrator' ||
    r === 'tenantadmin' ||
    r === 'systemadmin' ||
    r === '1' ||
    r === '2'
  )
}

export default function AdminRoute({ children }) {
  const token = tokenManager.getToken()
  const isAuthenticated = Boolean(token)

  if (!isAuthenticated) {
    return <Navigate to="/signin" replace />
  }

  const role = jwtHelper.getUserRole(token)

  if (!isAdminRole(role)) {
    return <Navigate to="/unauthorized" replace />
  }

  return children
}
