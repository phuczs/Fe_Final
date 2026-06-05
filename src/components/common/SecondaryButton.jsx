import { Button } from 'antd'

export default function SecondaryButton({
  children,
  icon,
  onClick,
}) {
  return (
    <Button
      icon={icon}
      onClick={onClick}
    >
      {children}
    </Button>
  )
}