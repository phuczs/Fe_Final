import api from './axios'

export const emailWhitelistApi = {
  // GET /api/email-whitelist — fetch all whitelisted emails (supports query params)
  list(params = {}) {
    return api.get('/api/email-whitelist', { params })
  },

  // POST /api/email-whitelist — add a new email to the whitelist
  create(payload) {
    return api.post('/api/email-whitelist', payload)
  },

  // PUT /api/email-whitelist/:id — update an existing whitelisted email
  update(id, payload) {
    return api.put(`/api/email-whitelist/${id}`, payload)
  },

  // DELETE /api/email-whitelist/:id — remove an email from the whitelist
  remove(id) {
    return api.delete(`/api/email-whitelist/${id}`)
  },
}
