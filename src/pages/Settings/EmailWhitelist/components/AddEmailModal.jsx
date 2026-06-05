import { useState } from 'react'
import { Form, Input, message } from 'antd'
import { MailOutlined } from '@ant-design/icons'
import { emailWhitelistApi } from '../../../../api/emailWhitelistApi'
import BaseDrawer from '../../../../components/common/BaseDrawer'

export default function AddEmailModal({ open, onClose, onSuccess }) {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)

  const handleConfirm = async () => {
    try {
      const values = await form.validateFields()
      setLoading(true)

      await emailWhitelistApi.create({
        Emails: [values.email.trim()],
      })

      message.success(`"${values.email.trim()}" added to whitelist.`)
      form.resetFields()
      onClose()
      onSuccess?.()
    } catch (err) {
      if (err?.errorFields) return // validation error — Ant Design handles display
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Failed to add email. Please try again.',
      )
    } finally {
      setLoading(false)
    }
  }

  const handleClose = () => {
    form.resetFields()
    onClose()
  }

  return (
    <BaseDrawer
      title="Add Email to Whitelist"
      open={open}
      onClose={handleClose}
      size="default"
      showFooter
      confirmText="Add Email"
      cancelText="Cancel"
      confirmLoading={loading}
      onConfirm={handleConfirm}
    >
      <Form
        form={form}
        layout="vertical"
        requiredMark="optional"
      >
        <Form.Item
          label="Email Address"
          name="email"
          rules={[
            { required: true, message: 'Please enter an email address.' },
            { type: 'email', message: 'Please enter a valid email address.' },
          ]}
        >
          <Input
            prefix={<MailOutlined style={{ color: '#667eea' }} />}
            placeholder="user@example.com"
            autoComplete="off"
          />
        </Form.Item>

        <Form.Item
          label="Note"
          name="note"
        >
          <Input.TextArea
            placeholder="Optional — describe why this email is whitelisted"
            rows={3}
            showCount
            maxLength={200}
          />
        </Form.Item>
      </Form>
    </BaseDrawer>
  )
}
