// src/api/authApi.js

import api from './axios'

export const authApi = {
  login(payload) {
    return api.post(
      '/api/auth/login',
      payload,
    )
  },

  register(payload) {
    return api.post(
      '/api/auth/register',
      payload,
    )
  },

  refresh() {
    return api.post(
      '/api/auth/refresh',
    )
  },

  logout() {
    return api.post(
      '/api/auth/logout',
    )
  },

  me() {
    return api.get(
      '/api/auth/me',
    )
  },

  verifyMfa(payload) {
    return api.post(
      '/api/auth/verify-mfa',
      payload,
    )
  },
}