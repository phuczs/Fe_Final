import { useState } from 'react'
import { message } from 'antd'
import { usersApi } from '../../api/usersApi'
import BaseModal from './BaseModal'

export default function DeleteUsersModal({
  open,
  onClose,
  selectedUserIds = [],
  onSuccess,
}) {
  const [loading, setLoading] = useState(false)

  const handleDelete = async () => {
    if (selectedUserIds.length === 0) {
      return
    }

    setLoading(true)

    try {
      await usersApi.delete({
        userIds: selectedUserIds,
      })

      message.success(
        `Successfully deleted ${selectedUserIds.length} user${
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
        'Unable to delete users at this time.'

      message.error(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  return (
    <BaseModal
      title="Delete users"
      message={`Are you sure you want to permanently delete the ${selectedUserIds.length} selected user${
        selectedUserIds.length > 1 ? 's' : ''
      }? This action cannot be undone.`}
      open={open}
      onClose={onClose}
      onConfirm={handleDelete}
      loading={loading}
      confirmText="Delete"
      cancelText="Cancel"
      isDanger
    />
  )
}
