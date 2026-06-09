import { useState } from 'react'
import { Form, Input, Button, Card, Typography, message, Result } from 'antd'
import { SendOutlined } from '@ant-design/icons'
import { emailWhitelistApi } from '../../api/emailWhitelistApi'
import './SendNotificationPage.css'

const { Title, Paragraph } = Typography

export default function SendNotificationPage() {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)
  const [result, setResult] = useState(null)

  const onFinish = async (values) => {
    try {
      setLoading(true)
      setResult(null)

      const response = await emailWhitelistApi.sendToAll({
        Subject: values.subject,
        Body: values.body,
      })

      const data = response.data

      message.success(`Successfully sent to ${data.sentCount} users!`)
      
      setResult({
        success: true,
        sentCount: data.sentCount,
        failedCount: data.failedTo?.length || 0,
      })

      form.resetFields()
    } catch (err) {
      console.error(err)
      message.error(
        err.response?.data?.message ||
        err.response?.data?.title ||
        'Failed to send notification.'
      )
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="send-notification-page">
      <div className="send-notification-header">
        <Title level={2}>Send Notification</Title>
        <Paragraph type="secondary">
          Compose a message to send to all eligible users in the tenant.
          This will also appear in their web Notification Bell.
        </Paragraph>
      </div>

      <Card className="send-notification-card">
        {result?.success && (
          <Result
            status="success"
            title={`Notification sent successfully!`}
            subTitle={`Delivered to ${result.sentCount} users. Failed to deliver to ${result.failedCount} users.`}
            extra={[
              <Button type="primary" key="console" onClick={() => setResult(null)}>
                Send Another
              </Button>,
            ]}
            style={{ padding: '24px 0' }}
          />
        )}

        {!result?.success && (
          <Form
            form={form}
            layout="vertical"
            onFinish={onFinish}
            disabled={loading}
          >
            <Form.Item
              label="Subject"
              name="subject"
              rules={[
                { required: true, message: 'Please enter a subject' },
                { max: 200, message: 'Subject cannot exceed 200 characters' }
              ]}
            >
              <Input placeholder="E.g. Important System Update" size="large" />
            </Form.Item>

            <Form.Item
              label="Message Body"
              name="body"
              rules={[{ required: true, message: 'Please enter the message body' }]}
            >
              <Input.TextArea
                placeholder="Write your message here..."
                rows={8}
                size="large"
              />
            </Form.Item>

            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                icon={<SendOutlined />}
                loading={loading}
                size="large"
                block
              >
                Send Notification to All Users
              </Button>
            </Form.Item>
          </Form>
        )}
      </Card>
    </div>
  )
}
