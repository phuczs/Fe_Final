import { useState } from 'react'
import { Form, Input, message } from 'antd'
import { MailOutlined, AlignLeftOutlined } from '@ant-design/icons'
import { emailWhitelistApi } from '../../../../api/emailWhitelistApi'
import BaseDrawer from '../../../../components/common/BaseDrawer'

export default function SendEmailModal({ open, onClose, onSuccess }) {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)

  const handleConfirm = async () => {
    try {
      const values = await form.validateFields()
      setLoading(true)

      const payload = {
        Subject: values.subject.trim(),
        Body: values.body.trim(),
      }

      const response = await emailWhitelistApi.sendToAll(payload)
      const data = response.data

      message.success(`Message sent successfully to ${data.sentCount || 0} whitelisted emails.`)
      
      if (data.failedTo && data.failedTo.length > 0) {
        message.warning(`Failed to send to ${data.failedTo.length} emails. Check logs for details.`)
      }

      form.resetFields()
      onClose()
      onSuccess?.()
    } catch (err) {
      if (err?.errorFields) return // validation error — Ant Design handles display
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Failed to send messages. Please try again.',
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
      title="Send Message to Whitelist"
      open={open}
      onClose={handleClose}
      size="default"
      showFooter
      confirmText="Send Message"
      cancelText="Cancel"
      confirmLoading={loading}
      onConfirm={handleConfirm}
    >
      <div style={{ marginBottom: 16, color: '#666' }}>
        This will send an email message to all active email addresses currently on the whitelist.
      </div>
      <Form
        form={form}
        layout="vertical"
        requiredMark="optional"
      >
        <Form.Item
          label="Subject"
          name="subject"
          rules={[
            { required: true, message: 'Please enter a subject.' },
            { max: 200, message: 'Subject cannot exceed 200 characters.' },
          ]}
        >
          <Input
            prefix={<MailOutlined style={{ color: '#667eea' }} />}
            placeholder="Message Subject"
            autoComplete="off"
          />
        </Form.Item>

        <Form.Item
          label="Message Body"
          name="body"
          rules={[
            { required: true, message: 'Please enter the message body.' },
          ]}
        >
          <Input.TextArea
            placeholder="Type your message here..."
            rows={6}
          />
        </Form.Item>
      </Form>
    </BaseDrawer>
  )
}
