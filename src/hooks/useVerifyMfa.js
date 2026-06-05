import { useState } from 'react'
import { authApi } from '../api/authApi'

export function useVerifyMfa() {
  const [loading, setLoading] =
    useState(false)

  const [error, setError] =
    useState('')

  const submitCode = async (
    code,
  ) => {
    try {
      setLoading(true)
      setError('')

      const tempToken =
        sessionStorage.getItem(
          'tempToken',
        )

      const response =
        await authApi.verifyMfa({
          tempToken,
          code,
        })

      return response.data
    } catch (error) {
      setError(
        error.response?.data?.message ||
          'Invalid verification code',
      )

      throw error
    } finally {
      setLoading(false)
    }
  }

  return {
    loading,
    error,
    submitCode,
  }
}