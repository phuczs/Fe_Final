import {
  Card,
  Checkbox,
  Form,
  Input,
  Switch,
  Typography,
} from 'antd'

import PrimaryButton from '../../../../components/common/PrimaryButton'
import SecondaryButton from '../../../../components/common/SecondaryButton'

const { Paragraph } = Typography

export default function TwoFactorAuthenticationForm() {
  const [form] = Form.useForm()

  return (
    <Card title="Two-factor authentication">
      <Form
        form={form}
        layout="vertical"
        initialValues={{
          enabled: true,
          methods: ['email'],
        }}
      >
        <Form.Item
          name="enabled"
          valuePropName="checked"
        >
          <Switch />
        </Form.Item>

        <Paragraph>
          Two-Factor Authentication (2FA) is a robust identity and access management (IAM) security method designed to add an extra layer of defense to your account. It requires users to provide two distinct forms of identification before gaining access to secure resources and data. In addition to standard user credentials (username and password), the system automatically generates a time-sensitive, 6-digit verification code that is sent directly to the user. This one-time passcode acts as a dynamic security key, ensuring that even if your password is compromised, unauthorized access remains blocked.
        </Paragraph>

        <Form.Item
          name="methods"
          label="Verification methods"
        >
          <Checkbox.Group>
            <div
              style={{
                display: 'flex',
                flexDirection: 'column',
                gap: 12,
              }}
            >
              <Checkbox value="email">
                Email
              </Checkbox>

              <Input
                placeholder="Enter email / user email address"
              />

              <Checkbox value="sms">
                SMS
              </Checkbox>

              <Input
                placeholder="Enter mobile phone number"
              />
            </div>
          </Checkbox.Group>
        </Form.Item>

        <div
          style={{
            display: 'flex',
            justifyContent: 'flex-end',
            gap: 8,
          }}
        >
          <SecondaryButton>
            Cancel
          </SecondaryButton>

          <PrimaryButton>
            Save
          </PrimaryButton>
        </div>
      </Form>
    </Card>
  )
}