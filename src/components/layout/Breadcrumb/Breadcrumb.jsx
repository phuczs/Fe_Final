import { useLocation } from 'react-router-dom'

export default function Breadcrumb() {
  const location = useLocation()

  const path = location.pathname
    .split('/')
    .filter(Boolean)
    .map(
      (item) =>
        item.charAt(0).toUpperCase() +
        item.slice(1)
    )

  return (
    <div className="breadcrumb">
      <span>Home</span>

      {path.map((item) => (
        <span key={item}>
          {' / '} {item}
        </span>
      ))}
    </div>
  )
}