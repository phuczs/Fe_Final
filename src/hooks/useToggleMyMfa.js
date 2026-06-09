import { useEffect, useState } from 'react'
import { authApi } from '../api/authApi'
import { usersApi } from '../api/usersApi'

export function useToggleMyMfa() {
  const [mfaEnabled, setMfaEnabled] =
    useState(false)

  const [loading, setLoading] =
    useState(false)

  const [fetching, setFetching] =
    useState(true)

  const [error, setError] =
    useState('')

  const [success, setSuccess] =
    useState(false)

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        setFetching(true)
        const response =
          await usersApi.getMyProfile()
        setMfaEnabled(
          response.data.mfaEnabled,
        )
      } catch {
        setError(
          'Failed to load MFA settings.',
        )
      } finally {
        setFetching(false)
      }
    }

    fetchProfile()
  }, [])

  const submitToggle = async (enabled) => {
    try {
      setLoading(true)
      setError('')
      setSuccess(false)

      const response =
        await authApi.toggleMyMfa({
          enabled,
        })

      setMfaEnabled(response.data.mfaEnabled)
      setSuccess(true)

      return response.data
    } catch (requestError) {
      const message =
        requestError.response?.data
          ?.message ||
        'Failed to update MFA settings.'

      setError(message)
      throw requestError
    } finally {
      setLoading(false)
    }
  }

  return {
    mfaEnabled,
    fetching,
    loading,
    error,
    success,
    submitToggle,
  }
}
