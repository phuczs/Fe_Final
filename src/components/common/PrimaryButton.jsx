import { Button } from 'antd'

export default function PrimaryButton({
  children,
  icon,
  onClick,
  loading,
  htmlType,
}) {
  return (
    <Button
      type="primary"
      icon={icon}
      onClick={onClick}
      loading={loading}
      htmlType={htmlType}
    >
      {children}
    </Button>
  )
}