const TOKEN_KEY = 'accessToken'

export const tokenManager = {
  getToken() {
    return localStorage.getItem(
      TOKEN_KEY,
    )
  },

  setToken(token) {
    localStorage.setItem(
      TOKEN_KEY,
      token,
    )
  },

  clearToken() {
    localStorage.removeItem(
      TOKEN_KEY,
    )
  },
}