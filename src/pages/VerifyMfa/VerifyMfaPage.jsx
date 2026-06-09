import { useState } from 'react'
import { Typography, message } from 'antd'
import { Link, useNavigate } from 'react-router-dom'
import { authApi } from '../../api/authApi'
import { tokenManager } from '../../utils/tokenManager'
import VerifyMfaForm from './components/VerifyMfaForm'

import '../../components/auth/AuthPages.css'
import './VerifyMfaPage.css'

export default function VerifyMfaPage() {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleVerify = async (code) => {
    setLoading(true)
    setError('')

    const tempToken = sessionStorage.getItem('tempToken')
    if (!tempToken) {
      setError('MFA session expired. Please sign in again.')
      message.error('MFA session expired. Please sign in again.')
      setLoading(false)
      navigate('/signin', { replace: true })
      return
    }

    try {
      const response = await authApi.verifyMfa({
        tempToken,
        code,
      })

      const accessToken = response.data?.accessToken

      if (accessToken) {
        tokenManager.setToken(accessToken)
        sessionStorage.removeItem('tempToken')
        message.success('Multi-factor authentication successful!')
        navigate('/home', { replace: true })
      } else {
        setError('Verification failed. Invalid response.')
      }
    } catch (err) {
      const msg =
        err.response?.data?.message ||
        err.response?.data?.title ||
        'Invalid or expired verification code. Please try again.'
      setError(msg)
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="auth-page auth-page--signin">
      <section className="auth-page__intro">
        <div className="auth-page__intro-card">
          <span className="auth-page__label">
            AvePoint MOS
          </span>

          <h1>
            Two-Factor Authentication
          </h1>

          <p>
            Enter the verification code sent
            to your registered email address.
          </p>
        </div>
      </section>

      <section className="auth-page__form-panel">
        <div className="auth-page__form-inner auth-page__form-inner--signin">
          <p className="auth-page__eyebrow">
            Security Verification
          </p>

          <h2>
            Verify your identity
          </h2>

          <p className="auth-page__description">
            This verification code expires in
            3 minutes.
          </p>

          <VerifyMfaForm
            onSubmit={handleVerify}
            loading={loading}
            error={error}
          />

          <Typography.Paragraph className="auth-card__footer">
            Wrong account?{' '}
            <Link to="/signin">
              Back to Sign In
            </Link>
          </Typography.Paragraph>
        </div>
      </section>
    </main>
  )
}