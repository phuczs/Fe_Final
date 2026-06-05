import { useNavigate } from 'react-router-dom'
import { Typography } from 'antd'
import { Link } from 'react-router-dom'

import avepointLogo from '../../assets/avepoint-logo.png'

import SignInForm from './components/SignInForm'

import { useSignIn } from '../../hooks/useSignIn'
import { tokenManager } from '../../utils/tokenManager'

import '../../components/auth/AuthPages.css'

function SignInPage() {
  const navigate = useNavigate()

  const {
    loading,
    error,
    submitSignIn,
  } = useSignIn()

  const handleSubmit = async (
    values,
  ) => {
    try {
      const response =
        await submitSignIn(values)

      if (response.mfaRequired) {
        sessionStorage.setItem(
          'tempToken',
          response.tempToken,
        )

        navigate('/verify-mfa', {
          replace: true,
        })

        return
      }

      if (response.accessToken) {
        tokenManager.setToken(
          response.accessToken,
        )
      }

      navigate('/home', {
        replace: true,
      })
    } catch {
      //
    }
  }

  return (
    <main className="auth-page auth-page--signin">
      <section
        className="auth-page__intro"
        aria-hidden="true"
      >
        <div className="auth-page__intro-card">
          <div className="auth-page__logo-wrap">
            <img
              className="auth-page__logo"
              src={avepointLogo}
              alt="AvePoint"
            />
          </div>

          <span className="auth-page__label">
            AvePoint MOS
          </span>

          <h1>
            Welcome to the management
            portal
          </h1>

          <p>
            AvePoint's MOS platform is
            the core management page and
            login entry point for its
            SaaS products.
          </p>
        </div>
      </section>

      <section className="auth-page__form-panel">
        <div className="auth-page__form-inner auth-page__form-inner--signin">
          <p className="auth-page__eyebrow">
            Sign in
          </p>

          <h2>
            Log in with email
          </h2>

          <p className="auth-page__description">
            Use your work email to access
            the assessment portal.
          </p>

          <SignInForm
            loading={loading}
            error={error}
            onSubmit={handleSubmit}
          />

          <Typography.Paragraph className="auth-card__footer">
            New here?{' '}
            <Link to="/register">
              Create an account
            </Link>
          </Typography.Paragraph>
        </div>
      </section>
    </main>
  )
}

export default SignInPage