import axios from 'axios'
import { tokenManager } from '../utils/tokenManager'

const API_URL = 'https://localhost:7270'

const api = axios.create({
  baseURL: API_URL,
  withCredentials: true,
})

api.interceptors.request.use(
  (config) => {
    const token = tokenManager.getToken()

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
)

api.interceptors.response.use(
  (response) => response,

  async (error) => {
    const originalRequest = error.config

    if (
      error.response?.status === 401 &&
      !originalRequest._retry
    ) {
      originalRequest._retry = true

      try {
        const refreshResponse =
          await axios.post(
            `${API_URL}/api/auth/refresh`,
            {},
            {
              withCredentials: true,
            },
          )

        const accessToken =
          refreshResponse.data.accessToken

        tokenManager.setToken(
          accessToken,
        )

        originalRequest.headers.Authorization =
          `Bearer ${accessToken}`

        return api(originalRequest)
      } catch {
        tokenManager.clearToken()

        window.location.href =
          '/signin'
      }
    }

    return Promise.reject(error)
  },
)

export default api