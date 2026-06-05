import {
  GoogleOutlined,
  MailOutlined,
  LockOutlined,
} from '@ant-design/icons'

import {
  Alert,
  Button,
  Checkbox,
  Divider,
  Form,
  Input,
  Typography,
} from 'antd'

import '../../../components/auth/AuthForm.css'
import './SignInForm.css'

export default function SignInForm({
  loading,
  error,
  onSubmit,
}) {
  return (
    <div className="sign-in-card auth-card">
      <Button
        className="sign-in-card__provider"
        size="large"
        block
      >
        <GoogleOutlined />
        Log in with Google
      </Button>

      <Divider className="sign-in-card__divider">
        or
      </Divider>

      <Form
        layout="vertical"
        requiredMark={false}
        onFinish={onSubmit}
      >
        <Form.Item
          name="email"
          label="Email"
          rules={[
            {
              required: true,
              message:
                'Please enter your email address.',
            },
            {
              type: 'email',
              message:
                'Please enter a valid email address.',
            },
          ]}
        >
          <Input
            prefix={<MailOutlined />}
            size="large"
            placeholder="name@napmail.cc"
            autoComplete="email"
          />
        </Form.Item>

        <Form.Item
          name="password"
          label="Password"
          rules={[
            {
              required: true,
              message:
                'Please enter your password.',
            },
          ]}
        >
          <Input.Password
            prefix={<LockOutlined />}
            size="large"
            placeholder="Enter your password"
            autoComplete="current-password"
          />
        </Form.Item>

        {error && (
          <Alert
            className="sign-in-card__error"
            type="error"
            showIcon
            message={error}
          />
        )}

        <div className="sign-in-card__meta-row">
          <Form.Item
            name="remember"
            valuePropName="checked"
            className="sign-in-card__remember"
          >
            <Checkbox>
              Stay signed in for the next 14
              days
            </Checkbox>
          </Form.Item>

          <Typography.Link href="/forgot-password">
            Forgot password?
          </Typography.Link>
        </div>

        <Button
          type="primary"
          htmlType="submit"
          size="large"
          block
          loading={loading}
        >
          Sign in
        </Button>
      </Form>
    </div>
  )
}