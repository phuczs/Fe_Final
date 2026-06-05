import api from './axios'

export const emailWhitelistApi = {
  // GET /api/email-whitelist — fetch all whitelisted emails (supports query params)
  list(params = {}) {
    return api.get('/api/whitelist', { params })
  },

  // POST /api/whitelist/emails — add a new email to the whitelist
  create(payload) {
    return api.post('/api/whitelist/emails', payload)
  },

  // PUT /api/email-whitelist/:id — update an existing whitelisted email
  update(id, payload) {
    return api.put(`/api/whitelist/${id}`, payload)
  },

  // DELETE /api/whitelist/emails/:id — remove an email from the whitelist
  remove(id) {
    return api.delete(`/api/whitelist/emails/${id}`)
  },

  // Toggle Whitelist
  toggleWhitelist(payload) {
    return api.put('/api/whitelist/toggle', payload)
  },

  // POST /api/whitelist/emails/send — send email to all whitelisted addresses
  sendToAll(payload) {
    return api.post('/api/whitelist/emails/send', payload)
  },
}
