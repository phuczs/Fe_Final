import { InfoCircleOutlined, LockOutlined, MailOutlined, PhoneOutlined, UserOutlined } from '@ant-design/icons'
import { Alert, Button, Col, Form, Input, Row, Select, Typography } from 'antd'
import '../../../components/auth/AuthForm.css'
import './RegisterForm.css'

// Password validation regex: at least 8 chars, 1 lowercase, 1 uppercase, 1 digit, 1 special character
const PASSWORD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':"|,.<>\/?]).{8,}$/

const validatePassword = (password) => {
  if (!password) return false
  return PASSWORD_REGEX.test(password)
}

const COUNTRY_CODE_DATA = [
  { value: '+65', country: 'Singapore', flag: 'sg' },
  { value: '+60', country: 'Malaysia', flag: 'my' },
  { value: '+84', country: 'Vietnam', flag: 'vn' },
  { value: '+66', country: 'Thailand', flag: 'th' },
  { value: '+62', country: 'Indonesia', flag: 'id' },
  { value: '+63', country: 'Philippines', flag: 'ph' },
  { value: '+81', country: 'Japan', flag: 'jp' },
  { value: '+82', country: 'Korea', flag: 'kr' },
  { value: '+86', country: 'China', flag: 'cn' },
  { value: '+1', country: 'United States', flag: 'us' },
  { value: '+44', country: 'United Kingdom', flag: 'gb' },
  { value: '+61', country: 'Australia', flag: 'au' },
]

const CountryCodeOption = ({ country, flag, value }) => (
  <span className="register-card__country-option" title={country}>
    <img
      className="register-card__country-flag"
      src={`https://flagcdn.com/w20/${flag}.png`}
      srcSet={`https://flagcdn.com/w40/${flag}.png 2x`}
      alt=" "
    />
    {value}
  </span>
)

const COUNTRY_CODES = COUNTRY_CODE_DATA.map(({ value, country, flag }) => ({
  value,
  label: <CountryCodeOption country={country} flag={flag} value={value} />,
}))

export default function RegisterForm({ loading, sendingCode, error, info, onSubmit, onSendCode }) {
  const [form] = Form.useForm()

  const handleSendCode = async () => {
    try {
      const { email, phoneNumber } = await form.validateFields(['email', 'phoneNumber'])
      await onSendCode({ email, phoneNumber })
    } catch {
      return
    }
  }

  return (
    <div className="register-card auth-card">
      <Typography.Text className="auth-card__note">
        <InfoCircleOutlined /> * Denotes a required field
      </Typography.Text>

      <Form form={form} layout="vertical" requiredMark={false} onFinish={onSubmit}>
        <Typography.Title level={4} className="auth-card__section-title">
          Provide your information
        </Typography.Title>

        <Row gutter={16}>
          <Col xs={12} md={12}>
            <Form.Item
              name="userId"
              label="User ID"
              rules={[
                { required: true, message: 'Please enter a User ID.' },
                { min: 4, message: 'User ID must have at least 4 characters.' },
                { pattern: /^[a-zA-Z0-9._-]+$/, message: 'User ID can only contain letters, numbers, and the characters . _ -' },
              ]}
            >
              <Input prefix={<UserOutlined />} size="large" placeholder="learner_01" />
            </Form.Item>
          </Col>

          <Col xs={12} md={12}>
            <Form.Item
              name="email"
              label="Email address"
              rules={[
                { required: true, message: 'Please enter your email address.' },
                { type: 'email', message: 'Please enter a valid email address.' },
              ]}
            >
              <Input prefix={<MailOutlined />} size="large" placeholder="name@domain.com" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item
          name="fullName"
          label="Full Name as per NRIC/FIN/Passport"
          rules={[
            { required: true, message: 'Please enter your full name.' },
            { min: 3, message: 'Full name must have at least 3 characters.' },
          ]}
        >
          <Input size="large" placeholder="Nguyen Van A" />
        </Form.Item>

        <Row gutter={16} className="register-card__contact-row">
          <Col xs={24} md={12}>
            <Form.Item
              name="password"
              label="Password"
              rules={[
                { required: true, message: 'Please enter your password.' },
                {
                  validator: (_, value) => {
                    if (!value) {
                      return Promise.resolve()
                    }
                    if (value.length < 8) {
                      return Promise.reject(new Error('Password must be at least 8 characters long.'))
                    }
                    if (!/[A-Z]/.test(value)) {
                      return Promise.reject(new Error('Password must contain at least one uppercase letter.'))
                    }
                    if (!/[a-z]/.test(value)) {
                      return Promise.reject(new Error('Password must contain at least one lowercase letter.'))
                    }
                    if (!/[0-9]/.test(value)) {
                      return Promise.reject(new Error('Password must contain at least one number.'))
                    }
                    if (!/[!@#$%^&*()_+\-=\[\]{};':"|,.<>\/?]/.test(value)) {
                      return Promise.reject(new Error('Password must contain at least one special character.'))
                    }
                    return Promise.resolve()
                  },
                },
              ]}
            >
              <Input.Password
                prefix={<LockOutlined />}
                size="large"
                placeholder="Create a password"
                autoComplete="new-password"
              />
            </Form.Item>
          </Col>

          <Col xs={24} md={12}>
            <Form.Item
              name="confirmPassword"
              label="Confirm password"
              dependencies={['password']}
              rules={[
                { required: true, message: 'Please confirm your password.' },
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    if (!value || getFieldValue('password') === value) {
                      return Promise.resolve()
                    }

                    return Promise.reject(new Error('The two passwords that you entered do not match.'))
                  },
                }),
              ]}
            >
              <Input.Password
                prefix={<LockOutlined />}
                size="large"
                placeholder="Repeat your password"
                autoComplete="new-password"
              />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col xs={24} md={12}>
            <Form.Item label="Phone number" required>
              <Input.Group compact>
                <Form.Item name="countryCode" initialValue="+65" noStyle>
                  <Select
                    className="register-card__country-code"
                    options={COUNTRY_CODES}
                    size="large"
                  />
                </Form.Item>
                <Form.Item
                  name="phoneNumber"
                  noStyle
                  rules={[
                    { required: true, message: 'Please enter your phone number.' },
                    { pattern: /^[0-9]{7,12}$/, message: 'Please enter a valid phone number.' },
                  ]}
                >
                  <Input
                    className="register-card__phone-input"
                    size="large"
                    placeholder="81234567"
                    prefix={<PhoneOutlined />}
                  />
                </Form.Item>
              </Input.Group>
            </Form.Item>
          </Col>

          <Col xs={24} md={12}>
            <Form.Item
              name="verificationCode"
              label="Security verification"
              className="register-card__verification-field"
              rules={[
                // { required: true, message: 'Please enter the verification code.' },
                { pattern: /^[0-9]{6}$/, message: 'The verification code must consist of 6 digits.' },
              ]}
            >
              <Input.Group compact>
                <Form.Item name="verificationCode" noStyle>
                  <Input size="large" placeholder="Enter code" className="register-card__verification-input" />
                </Form.Item>
                <Button
                  type="primary"
                  size="large"
                  className="register-card__code-button"
                  loading={sendingCode}
                  onClick={handleSendCode}
                >
                  Get verification code
                </Button>
              </Input.Group>
            </Form.Item>
          </Col>
        </Row>

        {info ? (
          <Alert className="register-card__info" type="info" showIcon message={info} />
        ) : null}

        {error ? (
          <Alert className="register-card__error" type="error" showIcon message={error} />
        ) : null}

        <Button type="primary" htmlType="submit" size="large" block loading={loading}>
          Submit
        </Button>
      </Form>
    </div>
  )
}
