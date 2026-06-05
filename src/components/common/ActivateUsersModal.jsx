import { useState } from 'react'
import { message } from 'antd'
import { usersApi } from '../../api/usersApi'
import BaseModal from './BaseModal'

export default function ActivateUsersModal({
  open,
  onClose,
  selectedUserIds = [],
  onSuccess,
}) {
  const [loading, setLoading] = useState(false)

  const handleActivate = async () => {
    if (selectedUserIds.length === 0) {
      return
    }

    setLoading(true)

    try {
      await usersApi.activate({
        userIds: selectedUserIds,
      })

      message.success(
        `Successfully activated ${selectedUserIds.length} user${
          selectedUserIds.length > 1 ? 's' : ''
        }`,
      )

      if (onSuccess) {
        onSuccess()
      }

      onClose()
    } catch (error) {
      const errorMessage =
        error.response?.data?.message ||
        error.response?.data?.title ||
        'Unable to activate users at this time.'

      message.error(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  return (
    <BaseModal
      title="Activate users"
      message={`Are you sure you want to activate the ${selectedUserIds.length} selected user${
        selectedUserIds.length > 1 ? 's' : ''
      }?`}
      open={open}
      onClose={onClose}
      onConfirm={handleActivate}
      loading={loading}
      confirmText="Activate"
      cancelText="No"
    />
  )
}
