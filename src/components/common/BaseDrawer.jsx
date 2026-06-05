import { Drawer, Space } from 'antd'

import PrimaryButton from './PrimaryButton'
import SecondaryButton from './SecondaryButton'

export default function BaseDrawer({
  title,
  open,
  onClose,
  width = 500,
  size,

  children,

  showFooter = false,
  confirmText = 'Save',
  cancelText = 'Cancel',

  confirmLoading = false,
  onConfirm,
  onCancel,
}) {
  const sizeMap = {
    small: 378,
    default: 500,
    large: 786,
  }

  const drawerWidth = size ? sizeMap[size] : width

  return (
    <Drawer
      title={title}
      placement="right"
      width={drawerWidth}
      open={open}
      onClose={onClose}
      destroyOnClose
      footer={
        showFooter && (
          <Space
            style={{
              width: '100%',
              justifyContent: 'flex-end',
            }}
          >
            <SecondaryButton onClick={() => {
              onCancel?.()
              onClose()
            }}>
              {cancelText}
            </SecondaryButton>

            <PrimaryButton
              loading={confirmLoading}
              onClick={onConfirm}
            >
              {confirmText}
            </PrimaryButton>
          </Space>
        )
      }
    >
      {children}
    </Drawer>
  )
}