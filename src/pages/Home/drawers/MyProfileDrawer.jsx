import { Avatar, Descriptions } from 'antd'

import BaseDrawer from '../../../components/common/BaseDrawer'

export default function MyProfileDrawer({
  open,
  onClose,
}) {
  return (
    <BaseDrawer
      title="My profile"
      open={open}
      onClose={onClose}
      width={500}
      showFooter
      confirmText="Edit"
      cancelText="Close"
    >
      <div
        style={{
          textAlign: 'center',
          marginBottom: 32,
        }}
      >
        <Avatar
          size={56}
          style={{
            background: '#8b0000',
            marginBottom: 12,
          }}
        >
          N
        </Avatar>

        <div
          style={{
            fontWeight: 600,
          }}
        >
          ncssdev@snapmail.cc
        </div>

        <div
          style={{
            color: '#8c8c8c',
            fontSize: 13,
          }}
        >
          Southeast Asia (Singapore)
        </div>
      </div>

      <Descriptions
        column={1}
        bordered
        size="small"
      >
        <Descriptions.Item label="Display name">
          ncssdev@snapmail.cc
        </Descriptions.Item>

        <Descriptions.Item label="User ID">
          ncssdev
        </Descriptions.Item>

        <Descriptions.Item label="Email address">
          yutolivo_pato@164.com
        </Descriptions.Item>

        <Descriptions.Item label="Staff & Student ID">
          -
        </Descriptions.Item>

        <Descriptions.Item label="Sex">
          Male
        </Descriptions.Item>

        <Descriptions.Item label="Mobile phone">
          -
        </Descriptions.Item>
      </Descriptions>
    </BaseDrawer>
  )
}