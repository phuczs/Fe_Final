import { Navigate } from 'react-router-dom'
import { tokenManager } from '../utils/tokenManager'

export default function ProtectedRoute({
  children,
}) {
  const isAuthenticated =
    Boolean(
      tokenManager.getToken(),
    )

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/signin"
        replace
      />
    )
  }

  return children
}