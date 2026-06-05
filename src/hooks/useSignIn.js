import { useState } from 'react'
import { authApi } from '../api/authApi'

export function useSignIn() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const submitSignIn = async ({
    email,
    password,
  }) => {
    try {
      setLoading(true)
      setError('')

      const response =
        await authApi.login({
          email,
          password,
        })

      return response.data
    } catch (requestError) {          
      const message =
        requestError.response?.data?.message ||
        requestError.response?.data?.title ||
        'Unable to sign in at this time.'

      setError(message)

      throw requestError
    } finally {
      setLoading(false)
    }
  }

  return {
    loading,
    error,
    submitSignIn,
  }
}