import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Avatar, Descriptions, Spin, Tag } from 'antd'

import { usersApi } from '../../../api/usersApi'
import BaseDrawer from '../../../components/common/BaseDrawer'

export default function MyProfileDrawer({ open, onClose }) {
  const navigate = useNavigate()
  const [profile, setProfile] = useState(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (!open) return

    const fetch = async () => {
      setLoading(true)
      try {
        const res = await usersApi.getMyProfile()
        setProfile(res.data)
      } catch {
        // silently fail — user will see dashes
      } finally {
        setLoading(false)
      }
    }

    fetch()
  }, [open])

  const initials = profile?.displayName
    ? profile.displayName
        .split(' ')
        .slice(0, 2)
        .map((w) => w[0]?.toUpperCase())
        .join('')
    : '?'

  const handleEdit = () => {
    onClose()
    navigate('/profile')
  }

  return (
    <BaseDrawer
      title="My Profile"
      open={open}
      onClose={onClose}
      width={500}
      showFooter
      confirmText="Edit Profile"
      cancelText="Close"
      onConfirm={handleEdit}
    >
      {loading ? (
        <div style={{ display: 'flex', justifyContent: 'center', padding: 48 }}>
          <Spin size="large" />
        </div>
      ) : (
        <>
          <div style={{ textAlign: 'center', marginBottom: 28 }}>
            <Avatar
              size={64}
              style={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                marginBottom: 12,
                fontSize: 24,
                fontWeight: 700,
              }}
            >
              {initials}
            </Avatar>

            <div style={{ fontWeight: 600, fontSize: 16 }}>
              {profile?.displayName || '—'}
            </div>

            <div style={{ color: '#8c8c8c', fontSize: 13, marginTop: 4 }}>
              {profile?.emailAddress || '—'}
            </div>

            <Tag
              color={profile?.mfaEnabled ? 'green' : 'default'}
              style={{ marginTop: 8 }}
            >
              MFA {profile?.mfaEnabled ? 'Enabled' : 'Disabled'}
            </Tag>
          </div>

          <Descriptions column={1} bordered size="small">
            <Descriptions.Item label="Display Name">
              {profile?.displayName || '—'}
            </Descriptions.Item>

            <Descriptions.Item label="Login ID">
              {profile?.userId || '—'}
            </Descriptions.Item>

            <Descriptions.Item label="Email Address">
              {profile?.emailAddress || '—'}
            </Descriptions.Item>

            <Descriptions.Item label="Staff / Student ID">
              {profile?.staffStudentId || '—'}
            </Descriptions.Item>

            <Descriptions.Item label="Sex">
              {profile?.sex
                ? profile.sex.charAt(0).toUpperCase() + profile.sex.slice(1)
                : '—'}
            </Descriptions.Item>

            <Descriptions.Item label="Mobile Phone">
              {profile?.mobilePhone || '—'}
            </Descriptions.Item>
          </Descriptions>
        </>
      )}
    </BaseDrawer>
  )
}