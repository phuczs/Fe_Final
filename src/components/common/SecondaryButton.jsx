import { Button } from 'antd'

export default function SecondaryButton({
  children,
  icon,
  onClick,
  disabled,
}) {
  return (
    <Button
      icon={icon}
      onClick={onClick}
      disabled={disabled}
    >
      {children}
    </Button>
  )
}