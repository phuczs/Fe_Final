import api from './axios'

export const usersApi = {
  list(params = {}) {
    return api.get('/api/users', { params })
  },
  create(payload) {
    return api.post('/api/users', payload)
  },

  activate(payload) {
    return api.patch('/api/users/activate', payload)
  },

  deactivate(payload) {
    return api.patch('/api/users/deactivate', payload)
  },

  delete(payload) {
    return api.delete('/api/users', {
      data: payload,
    })
  },

  // GET /api/users/me — fetch the current logged-in user's profile
  getMyProfile() {
    return api.get('/api/users/me')
  },

  // PUT /api/users/me — update display name, sex, phone, staffId
  updateMyProfile(payload) {
    return api.put('/api/users/me', payload)
  },

  // POST /api/users/me/reset-password — change password
  resetPassword(payload) {
    return api.post('/api/users/me/reset-password', payload)
  },
}