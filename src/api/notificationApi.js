import api from './axios'

export const notificationApi = {
  getNotifications() {
    return api.get('/api/notifications')
  },
  markAsRead(id) {
    return api.post(`/api/notifications/${id}/read`)
  },
  markAllAsRead() {
    return api.post('/api/notifications/read-all')
  },
}
