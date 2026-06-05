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
}