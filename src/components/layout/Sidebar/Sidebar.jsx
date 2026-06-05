import { NavLink } from 'react-router-dom'
import { navItems } from './navItems'

import './Sidebar.css'

export default function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="sidebar__logo">
        MOS
      </div>

      <nav className="sidebar__nav">
        {navItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) =>
              isActive
                ? 'sidebar__item sidebar__item--active'
                : 'sidebar__item'
            }
          >
            <span className="sidebar__icon">
              {item.icon}
            </span>

            <span className="sidebar__label">
              {item.label}
            </span>
          </NavLink>
        ))}
      </nav>
    </aside>
  )
}