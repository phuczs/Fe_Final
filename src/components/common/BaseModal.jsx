import { Modal } from 'antd'
import PrimaryButton from './PrimaryButton'
import SecondaryButton from './SecondaryButton'

export default function BaseModal({
  title,
  message,
  open,
  onClose,
  onConfirm,
  loading = false,
  confirmText = 'Confirm',
  cancelText = 'Cancel',
  isDanger = false,
}) {
  return (
    <Modal
      title={title}
      open={open}
      onCancel={onClose}
      footer={[
        <SecondaryButton key="cancel" onClick={onClose}>
          {cancelText}
        </SecondaryButton>,
        <PrimaryButton
          key="confirm"
          loading={loading}
          onClick={onConfirm}
        >
          {confirmText}
        </PrimaryButton>,
      ]}
    >
      {message}
    </Modal>
  )
}
