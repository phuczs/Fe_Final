export const jwtHelper = {
  decodeToken(token) {
    if (!token) return null
    try {
      const base64Url = token.split('.')[1]
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      )
      return JSON.parse(jsonPayload)
    } catch (error) {
      console.error('Failed to decode JWT token', error)
      return null
    }
  },

  getUserRole(token) {
    const decoded = this.decodeToken(token)
    if (!decoded) return null

    return (
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      decoded.role ||
      decoded.Role ||
      null
    )
  }
}
