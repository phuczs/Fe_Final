import { useEffect, useState } from 'react'
import { Form, Input, message } from 'antd'
import { MailOutlined } from '@ant-design/icons'
import { emailWhitelistApi } from '../../../../api/emailWhitelistApi'
import BaseDrawer from '../../../../components/common/BaseDrawer'

export default function EditEmailModal({ open, record, onClose, onSuccess }) {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)

  // Pre-fill form when a record is selected
  useEffect(() => {
    if (open && record) {
      form.setFieldsValue({
        email: record.email !== '-' ? record.email : '',
        note: record.note !== '-' ? record.note : '',
      })
    }
  }, [open, record, form])

  const handleConfirm = async () => {
    try {
      const values = await form.validateFields()
      setLoading(true)

      await emailWhitelistApi.update(record.id, {
        email: values.email.trim(),
        note: values.note?.trim() ?? '',
      })

      message.success('Email whitelist entry updated successfully.')
      onClose()
      onSuccess?.()
    } catch (err) {
      if (err?.errorFields) return // validation — Ant Design handles display
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Failed to update email. Please try again.',
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
      title="Edit Whitelisted Email"
      open={open}
      onClose={handleClose}
      size="default"
      showFooter
      confirmText="Save Changes"
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
