import { useState } from 'react'
import { message } from 'antd'
import { usersApi } from '../../api/usersApi'
import BaseModal from './BaseModal'

export default function DeactivateUsersModal({
  open,
  onClose,
  selectedUserIds = [],
  onSuccess,
}) {
  const [loading, setLoading] = useState(false)

  const handleDeactivate = async () => {
    if (selectedUserIds.length === 0) {
      return
    }

    setLoading(true)

    try {
      await usersApi.deactivate({
        userIds: selectedUserIds,
      })

      message.success(
        `Successfully deactivated ${selectedUserIds.length} user${
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
        'Unable to deactivate users at this time.'

      message.error(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  return (
    <BaseModal
      title="Deactivate users"
      message={`Are you sure you want to deactivate the ${selectedUserIds.length} selected user${
        selectedUserIds.length > 1 ? 's' : ''
      }?`}
      open={open}
      onClose={onClose}
      onConfirm={handleDeactivate}
      loading={loading}
      confirmText="Deactivate"
      cancelText="No"
    />
  )
}
