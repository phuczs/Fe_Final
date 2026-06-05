import { useState } from 'react'
import { message } from 'antd'
import { emailWhitelistApi } from '../../../../api/emailWhitelistApi'
import BaseModal from '../../../../components/common/BaseModal'

export default function DeleteEmailModal({ open, record, onClose, onSuccess }) {
  const [loading, setLoading] = useState(false)

  const handleDelete = async () => {
    if (!record?.id) return

    setLoading(true)
    try {
      await emailWhitelistApi.remove(record.id)
      message.success(`"${record.email}" removed from whitelist.`)
      onClose()
      onSuccess?.()
    } catch (err) {
      message.error(
        err.response?.data?.message ||
          err.response?.data?.title ||
          'Failed to delete email. Please try again.',
      )
    } finally {
      setLoading(false)
    }
  }

  return (
    <BaseModal
      title="Remove from Whitelist"
      message={
        record
          ? `Are you sure you want to remove "${record.email}" from the email whitelist? This action cannot be undone.`
          : 'Are you sure you want to remove this entry?'
      }
      open={open}
      onClose={onClose}
      onConfirm={handleDelete}
      loading={loading}
      confirmText="Remove"
      cancelText="Cancel"
      isDanger
    />
  )
}
