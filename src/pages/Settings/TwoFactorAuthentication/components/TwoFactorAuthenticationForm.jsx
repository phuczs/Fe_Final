import {
  Alert,
  Card,
  Form,
  Skeleton,
  Switch,
  Typography,
} from 'antd'

import PrimaryButton from '../../../../components/common/PrimaryButton'
import SecondaryButton from '../../../../components/common/SecondaryButton'

import { useToggleMyMfa } from '../../../../hooks/useToggleMyMfa'

const { Paragraph } = Typography

export default function TwoFactorAuthenticationForm() {
  const [form] = Form.useForm()

  const {
    mfaEnabled,
    fetching,
    loading,
    error,
    success,
    submitToggle,
  } = useToggleMyMfa()

  const handleFinish = async (values) => {
    try {
      await submitToggle(values.enabled)
    } catch {
      //
    }
  }

  const handleCancel = () => {
    form.setFieldsValue({ enabled: mfaEnabled })
  }

  if (fetching) {
    return (
      <Card title="Two-factor authentication">
        <Skeleton active />
      </Card>
    )
  }

  return (
    <Card title="Two-factor authentication">
      <Form
        form={form}
        layout="vertical"
        initialValues={{ enabled: mfaEnabled }}
        onFinish={handleFinish}
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

        {error && (
          <Alert
            type="error"
            message={error}
            showIcon
            style={{ marginBottom: 16 }}
          />
        )}

        {success && (
          <Alert
            type="success"
            message="MFA settings updated successfully."
            showIcon
            style={{ marginBottom: 16 }}
          />
        )}

        <div
          style={{
            display: 'flex',
            justifyContent: 'flex-end',
            gap: 8,
          }}
        >
          <SecondaryButton
            onClick={handleCancel}
            disabled={loading}
          >
            Cancel
          </SecondaryButton>

          <PrimaryButton
            htmlType="submit"
            loading={loading}
          >
            Save
          </PrimaryButton>
        </div>
      </Form>
    </Card>
  )
}