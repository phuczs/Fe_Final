import api from './axios'

export const auditApi = {
  list(params = {}) {
    return api.get('/api/audit', { params })
  },

  export(params = {}) {
    return api.get('/api/audit/export', {
      params,
      responseType: 'blob',
    })
  },
}