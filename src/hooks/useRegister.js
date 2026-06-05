import { useState } from 'react'
import { authApi } from '../api/authApi'

function delay(duration) {
  return new Promise((resolve) => {
    window.setTimeout(resolve, duration)
  })
}

function createVerificationCode() {
  return String(Math.floor(100000 + Math.random() * 900000))
}

const validatePassword = (password) => {
  if (!password || password.length < 8) return false
  if (!/[A-Z]/.test(password)) return false
  if (!/[a-z]/.test(password)) return false
  if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) return false
  return true
}

export function useRegister() {
  const [submitting, setSubmitting] = useState(false)
  const [sendingCode, setSendingCode] = useState(false)
  const [error, setError] = useState('')
  const [info, setInfo] = useState('')
  const [verificationCode, setVerificationCode] = useState('')

  const sendVerificationCode = async ({ phoneNumber }) => {
    setSendingCode(true)
    setError('')
    setInfo('')

    try {
      if (!phoneNumber) {
        throw new Error('Please enter your phone number before requesting a verification code.')
      }

      await delay(700)

      // const nextCode = createVerificationCode()
      const nextCode = '123456' // For demo purposes, use a fixed code
      setVerificationCode(nextCode)
      setInfo(`Verification code sent. Demo code: ${nextCode}`)

      return nextCode
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : 'Cannot send verification code at this time.',
      )
      throw requestError
    } finally {
      setSendingCode(false)
    }
  }

  const submitRegister = async (values) => {
    setSubmitting(true)
    setError('')

    try {
      if (!validatePassword(values.password)) {
        throw new Error('Password must be at least 8 characters long, contain at least one lowercase letter, at least one uppercase letter, and at least one special character.')
      }

      const response = await authApi.register({
        userId: values.userId,
        emailAddress: values.email,
        fullName: values.fullName,
        password: values.password,
        confirmPassword: values.confirmPassword,
        countryCode: values.countryCode,
        phoneNumber: values.phoneNumber,
        securityVerificationCode: values.verificationCode,
      })

      return response.data
    } catch (requestError) {
      const validationErrors = requestError.response?.data?.errors
      const passwordValidationMessage = Array.isArray(validationErrors?.Password)
        ? validationErrors.Password[0]
        : undefined

      const message =
        passwordValidationMessage ||
        requestError.response?.data?.message ||
        requestError.response?.data?.title ||
        requestError.message ||
        'Cannot register at this time.'

      setError(message)
      throw requestError
    } finally {
      setSubmitting(false)
    }
  }

  return {
    submitting,
    sendingCode,
    error,
    info,
    sendVerificationCode,
    submitRegister,
  }
}