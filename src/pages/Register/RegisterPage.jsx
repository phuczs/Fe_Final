import { useNavigate } from 'react-router-dom'
import { Link } from 'react-router-dom'
import { Typography } from 'antd'
import RegisterForm from './components/RegisterForm.jsx'
import { useAuthSession } from '../../hooks/useAuthSession'
import { useRegister } from '../../hooks/useRegister'
import '../../components/auth/AuthPages.css'

function RegisterPage() {
  const navigate = useNavigate()
  const { signIn } = useAuthSession()
  const { submitting, sendingCode, error, info, sendVerificationCode, submitRegister } = useRegister()

  const handleSubmit = async (values) => {
    try {
      await submitRegister(values)
      signIn()
      navigate('/dashboard', { replace: true })
    } catch {
      return
    }
  }

  return (
    <main className="auth-page auth-page--register auth-page--centered">
      <section className="auth-page__form-panel">
        <div className="auth-page__form-inner auth-page__form-inner--register">
          <p className="auth-page__eyebrow">Register</p>
          <h2>Register as a Learner</h2>
          <p className="auth-page__description">
            To register as a learner you need to fill in the following information.
          </p>
          <RegisterForm
            loading={submitting}
            sendingCode={sendingCode}
            error={error}
            info={info}
            onSubmit={handleSubmit}
            onSendCode={sendVerificationCode}
          />
          <Typography.Paragraph className="auth-card__footer">
            Already have an account? <Link to="/signin">Sign in</Link>
          </Typography.Paragraph>
        </div>
      </section>
    </main>
  )
}

export default RegisterPage
