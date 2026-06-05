import {
  Alert,
  Button,
  Form,
  Input,
} from 'antd'
import {
  SafetyCertificateOutlined,
} from '@ant-design/icons'

import { useEffect, useState } from 'react'

export default function VerifyMfaForm({
  onSubmit,
}) {
  const [seconds, setSeconds] =
    useState(180)

  const [error] = useState('')

  useEffect(() => {
    const timer = setInterval(() => {
      setSeconds((prev) => {
        if (prev <= 1) {
          clearInterval(timer)
          return 0
        }

        return prev - 1
      })
    }, 1000)

    return () => clearInterval(timer)
  }, [])

  const formatTime = () => {
    const mins = String(
      Math.floor(seconds / 60),
    ).padStart(2, '0')

    const secs = String(
      seconds % 60,
    ).padStart(2, '0')

    return `${mins}:${secs}`
  }

  return (
    <div className="verify-mfa-card auth-card">
      <Form
        layout="vertical"
        onFinish={(values) =>
          onSubmit(values.code)
        }
      >
        <Form.Item
          label="Verification Code"
          name="code"
          rules={[
            {
              required: true,
              message:
                'Please enter the verification code.',
            },
            {
              pattern: /^\d{6}$/,
              message:
                'Verification code must be 6 digits.',
            },
          ]}
        >
          <Input
            prefix={
              <SafetyCertificateOutlined />
            }
            placeholder="Enter 6-digit code"
            size="large"
            maxLength={6}
          />
        </Form.Item>

        <div className="verify-mfa__timer">
          Code expires in{' '}
          <strong>
            {formatTime()}
          </strong>
        </div>

        {error && (
          <Alert
            type="error"
            showIcon
            message={error}
            style={{
              marginBottom: 16,
            }}
          />
        )}

        <Button
          type="primary"
          htmlType="submit"
          size="large"
          block
        >
          Verify
        </Button>

        <Button
          block
          style={{
            marginTop: 12,
          }}
          disabled={seconds > 0}
        >
          Resend Code
        </Button>
      </Form>
    </div>
  )
}