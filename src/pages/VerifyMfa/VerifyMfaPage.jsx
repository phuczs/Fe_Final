import { Typography } from 'antd'
import { Link, useNavigate } from 'react-router-dom'

import VerifyMfaForm from './components/VerifyMfaForm'

import '../../components/auth/AuthPages.css'
import './VerifyMfaPage.css'

export default function VerifyMfaPage() {
  const navigate = useNavigate()

  const handleVerify = (code) => {
    console.log('Verify MFA:', code)

    navigate('/home')
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