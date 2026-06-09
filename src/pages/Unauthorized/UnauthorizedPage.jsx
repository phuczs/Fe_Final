import { Button, Typography } from 'antd'
import { LockOutlined } from '@ant-design/icons'
import { useNavigate } from 'react-router-dom'
import './UnauthorizedPage.css'

const { Title, Paragraph } = Typography

export default function UnauthorizedPage() {
  const navigate = useNavigate()

  return (
    <div className="unauthorized-page">
      <div className="unauthorized-card">
        <div className="unauthorized-icon-wrap">
          <LockOutlined />
        </div>
        <Title level={3} className="unauthorized-title">
          Access Denied
        </Title>
        <Paragraph className="unauthorized-subtitle">
          You are not authorized to view this page. If you believe this is an error, please contact your administrator.
        </Paragraph>
        <Button
          type="primary"
          size="large"
          className="unauthorized-btn"
          onClick={() => navigate('/home')}
        >
          Back to Home
        </Button>
      </div>
    </div>
  )
}
