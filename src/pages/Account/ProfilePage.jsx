import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Avatar,
  Button,
  Divider,
  Form,
  Input,
  Select,
  Spin,
  Tabs,
  Tag,
  message,
} from 'antd'
import {
  UserOutlined,
  LockOutlined,
  EditOutlined,
  SaveOutlined,
  CloseOutlined,
  MailOutlined,
  PhoneOutlined,
  IdcardOutlined,
  ArrowLeftOutlined,
} from '@ant-design/icons'
import { usersApi } from '../../api/usersApi'
import './ProfilePage.css'

const { TabPane } = Tabs

export default function ProfilePage() {
  const navigate = useNavigate()
  const [profile, setProfile] = useState(null)
  const [loadingProfile, setLoadingProfile] = useState(true)
  const [editing, setEditing] = useState(false)
  const [savingProfile, setSavingProfile] = useState(false)
  const [savingPassword, setSavingPassword] = useState(false)

  const [profileForm] = Form.useForm()
  const [passwordForm] = Form.useForm()

  // ── load profile ──────────────────────────────────────────────────────────
  const loadProfile = async () => {
    setLoadingProfile(true)
    try {
      const res = await usersApi.getMyProfile()
      setProfile(res.data)
      profileForm.setFieldsValue({
        displayName: res.data.displayName,
        staffStudentId: res.data.staffStudentId ?? '',
        sex: res.data.sex ?? '',
        mobilePhone: res.data.mobilePhone ?? '',
      })
    } catch (err) {
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Failed to load profile.',
      )
    } finally {
      setLoadingProfile(false)
    }
  }

  useEffect(() => {
    loadProfile()
  }, [])

  // ── update profile ────────────────────────────────────────────────────────
  const handleSaveProfile = async () => {
    try {
      const values = await profileForm.validateFields()
      setSavingProfile(true)
      const res = await usersApi.updateMyProfile({
        DisplayName: values.displayName?.trim() || null,
        StaffStudentId: values.staffStudentId?.trim() || null,
        Sex: values.sex || null,
        MobilePhone: values.mobilePhone?.trim() || null,
      })
      setProfile((prev) => ({ ...prev, ...res.data }))
      message.success('Profile updated successfully.')
      setEditing(false)
    } catch (err) {
      if (err?.errorFields) return
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Failed to update profile.',
      )
    } finally {
      setSavingProfile(false)
    }
  }

  const handleCancelEdit = () => {
    profileForm.setFieldsValue({
      displayName: profile?.displayName,
      staffStudentId: profile?.staffStudentId ?? '',
      sex: profile?.sex ?? '',
      mobilePhone: profile?.mobilePhone ?? '',
    })
    setEditing(false)
  }

  // ── reset password ────────────────────────────────────────────────────────
  const handleResetPassword = async () => {
    try {
      const values = await passwordForm.validateFields()
      setSavingPassword(true)
      await usersApi.resetPassword({
        CurrentPassword: values.currentPassword,
        NewPassword: values.newPassword,
        ConfirmNewPassword: values.confirmNewPassword,
      })
      message.success('Password changed successfully.')
      passwordForm.resetFields()
    } catch (err) {
      if (err?.errorFields) return
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Failed to change password.',
      )
    } finally {
      setSavingPassword(false)
    }
  }

  // ── avatar initials ───────────────────────────────────────────────────────
  const initials = profile?.displayName
    ? profile.displayName
        .split(' ')
        .slice(0, 2)
        .map((w) => w[0]?.toUpperCase())
        .join('')
    : '?'

  if (loadingProfile) {
    return (
      <div className="profile-loading">
        <Spin size="large" />
      </div>
    )
  }

  return (
    <div className="profile-page">
      {/* ── back button ───────────────────────────────────────────── */}
      <div className="profile-back">
        <Button
          icon={<ArrowLeftOutlined />}
          onClick={() => navigate(-1)}
          className="profile-btn-back"
        >
          Back
        </Button>
      </div>

      {/* ── banner ───────────────────────────────────────────────────── */}
      <div className="profile-banner">
        <div className="profile-banner__avatar">
          <Avatar size={88} className="profile-avatar">
            {initials}
          </Avatar>
          <div className="profile-banner__info">
            <h1 className="profile-banner__name">{profile?.displayName || '—'}</h1>
            <div className="profile-banner__meta">
              <span className="profile-banner__email">
                <MailOutlined style={{ marginRight: 6 }} />
                {profile?.emailAddress || '—'}
              </span>
              <Tag color="geekblue" className="profile-banner__mfa">
                MFA {profile?.mfaEnabled ? 'Enabled' : 'Disabled'}
              </Tag>
            </div>
          </div>
        </div>
      </div>

      {/* ── tabs ─────────────────────────────────────────────────────── */}
      <div className="profile-body">
        <Tabs defaultActiveKey="profile" className="profile-tabs">

          {/* Profile tab */}
          <TabPane
            tab={<span><UserOutlined /> Profile</span>}
            key="profile"
          >
            <div className="profile-section">
              <div className="profile-section__header">
                <div>
                  <h2 className="profile-section__title">Personal Information</h2>
                  <p className="profile-section__subtitle">
                    Update your display name, contact details and other info.
                  </p>
                </div>
                {!editing ? (
                  <Button
                    icon={<EditOutlined />}
                    onClick={() => setEditing(true)}
                    className="profile-btn-edit"
                  >
                    Edit Profile
                  </Button>
                ) : (
                  <div style={{ display: 'flex', gap: 8 }}>
                    <Button
                      icon={<CloseOutlined />}
                      onClick={handleCancelEdit}
                    >
                      Cancel
                    </Button>
                    <Button
                      type="primary"
                      icon={<SaveOutlined />}
                      loading={savingProfile}
                      onClick={handleSaveProfile}
                      className="profile-btn-save"
                    >
                      Save Changes
                    </Button>
                  </div>
                )}
              </div>

              <Divider />

              {/* Read-only fields */}
              <div className="profile-readonly-grid">
                <div className="profile-readonly-item">
                  <span className="profile-readonly-label">
                    <MailOutlined /> Email Address
                  </span>
                  <span className="profile-readonly-value">{profile?.emailAddress || '—'}</span>
                </div>
                <div className="profile-readonly-item">
                  <span className="profile-readonly-label">
                    <IdcardOutlined /> Login ID
                  </span>
                  <span className="profile-readonly-value">{profile?.userId || '—'}</span>
                </div>
              </div>

              <Divider />

              {/* Editable fields */}
              <Form
                form={profileForm}
                layout="vertical"
                disabled={!editing}
                className="profile-form"
              >
                <div className="profile-form-grid">
                  <Form.Item
                    label="Display Name"
                    name="displayName"
                    rules={[{ required: true, message: 'Display name is required.' }]}
                  >
                    <Input prefix={<UserOutlined />} placeholder="Enter display name" />
                  </Form.Item>

                  <Form.Item label="Staff / Student ID" name="staffStudentId">
                    <Input prefix={<IdcardOutlined />} placeholder="e.g. S1234567" />
                  </Form.Item>

                  <Form.Item label="Mobile Phone" name="mobilePhone">
                    <Input prefix={<PhoneOutlined />} placeholder="+65 9123 4567" />
                  </Form.Item>

                  <Form.Item label="Sex" name="sex">
                    <Select placeholder="Select sex" allowClear>
                      <Select.Option value="male">Male</Select.Option>
                      <Select.Option value="female">Female</Select.Option>
                    </Select>
                  </Form.Item>
                </div>
              </Form>
            </div>
          </TabPane>

          {/* Security tab */}
          <TabPane
            tab={<span><LockOutlined /> Security</span>}
            key="security"
          >
            <div className="profile-section">
              <div className="profile-section__header">
                <div>
                  <h2 className="profile-section__title">Change Password</h2>
                  <p className="profile-section__subtitle">
                    Choose a strong password you haven't used before.
                  </p>
                </div>
              </div>

              <Divider />

              <Form
                form={passwordForm}
                layout="vertical"
                className="profile-form profile-form--narrow"
              >
                <Form.Item
                  label="Current Password"
                  name="currentPassword"
                  rules={[{ required: true, message: 'Please enter your current password.' }]}
                >
                  <Input.Password
                    prefix={<LockOutlined />}
                    placeholder="Current password"
                    autoComplete="current-password"
                  />
                </Form.Item>

                <Form.Item
                  label="New Password"
                  name="newPassword"
                  rules={[
                    { required: true, message: 'Please enter a new password.' },
                    { min: 8, message: 'Password must be at least 8 characters.' },
                  ]}
                >
                  <Input.Password
                    prefix={<LockOutlined />}
                    placeholder="New password"
                    autoComplete="new-password"
                  />
                </Form.Item>

                <Form.Item
                  label="Confirm New Password"
                  name="confirmNewPassword"
                  dependencies={['newPassword']}
                  rules={[
                    { required: true, message: 'Please confirm your new password.' },
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (!value || getFieldValue('newPassword') === value) {
                          return Promise.resolve()
                        }
                        return Promise.reject(new Error('Passwords do not match.'))
                      },
                    }),
                  ]}
                >
                  <Input.Password
                    prefix={<LockOutlined />}
                    placeholder="Confirm new password"
                    autoComplete="new-password"
                  />
                </Form.Item>

                <Button
                  type="primary"
                  loading={savingPassword}
                  onClick={handleResetPassword}
                  className="profile-btn-save"
                  style={{ marginTop: 8 }}
                >
                  Change Password
                </Button>
              </Form>
            </div>
          </TabPane>
        </Tabs>
      </div>
    </div>
  )
}
