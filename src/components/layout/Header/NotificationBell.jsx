import React, { useState, useEffect } from 'react'
import { Badge, Popover, List, Typography, Button, Spin, Empty, message } from 'antd'
import { BellOutlined, CheckOutlined } from '@ant-design/icons'
import { notificationApi } from '../../../api/notificationApi'
import './NotificationBell.css'

const { Text } = Typography

export default function NotificationBell() {
  const [notifications, setNotifications] = useState([])
  const [loading, setLoading] = useState(false)
  const [visible, setVisible] = useState(false)

  const fetchNotifications = async () => {
    try {
      setLoading(true)
      const res = await notificationApi.getNotifications()
      setNotifications(res.data || [])
    } catch (err) {
      console.error('Failed to load notifications', err)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchNotifications()
    // Polling every 60 seconds
    const interval = setInterval(fetchNotifications, 60000)
    return () => clearInterval(interval)
  }, [])

  const handleOpenChange = (newVisible) => {
    setVisible(newVisible)
    if (newVisible) {
      fetchNotifications()
    }
  }

  const markAsRead = async (id) => {
    try {
      await notificationApi.markAsRead(id)
      setNotifications(prev =>
        prev.map(n => n.id === id ? { ...n, isRead: true } : n)
      )
    } catch (err) {
      message.error('Failed to mark notification as read.')
    }
  }

  const markAllAsRead = async () => {
    try {
      await notificationApi.markAllAsRead()
      setNotifications(prev => prev.map(n => ({ ...n, isRead: true })))
      message.success('All notifications marked as read.')
    } catch (err) {
      message.error('Failed to mark all as read.')
    }
  }

  const unreadCount = notifications.filter(n => !n.isRead).length

  const content = (
    <div className="notification-popover-content">
      <div className="notification-header">
        <Text strong>Notifications</Text>
        {unreadCount > 0 && (
          <Button type="link" size="small" onClick={markAllAsRead}>
            Mark all as read
          </Button>
        )}
      </div>
      <div className="notification-list-container">
        {loading && notifications.length === 0 ? (
          <div className="notification-loading">
            <Spin />
          </div>
        ) : notifications.length === 0 ? (
          <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} description="No notifications" />
        ) : (
          <List
            itemLayout="horizontal"
            dataSource={notifications}
            renderItem={item => (
              <List.Item
                className={`notification-item ${!item.isRead ? 'unread' : ''}`}
                onClick={() => !item.isRead && markAsRead(item.id)}
                actions={[
                  !item.isRead && (
                    <Button
                      type="text"
                      icon={<CheckOutlined />}
                      size="small"
                      title="Mark as read"
                      onClick={(e) => {
                        e.stopPropagation();
                        markAsRead(item.id);
                      }}
                    />
                  )
                ]}
              >
                <List.Item.Meta
                  title={
                    <Text strong={!item.isRead} ellipsis={{ tooltip: item.subject }}>
                      {item.subject}
                    </Text>
                  }
                  description={
                    <div>
                      <div className="notification-body">{item.body}</div>
                      <div className="notification-time">
                        {new Date(item.sentAt).toLocaleString()}
                      </div>
                    </div>
                  }
                />
              </List.Item>
            )}
          />
        )}
      </div>
    </div>
  )

  return (
    <Popover
      content={content}
      trigger="click"
      open={visible}
      onOpenChange={handleOpenChange}
      placement="bottomRight"
      overlayClassName="notification-popover"
    >
      <Badge count={unreadCount} size="small" className="notification-badge">
        <Button
          type="text"
          icon={<BellOutlined style={{ fontSize: '18px' }} />}
          className="notification-btn"
        />
      </Badge>
    </Popover>
  )
}
