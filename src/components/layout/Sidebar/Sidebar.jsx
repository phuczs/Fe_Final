import { NavLink } from 'react-router-dom'
import { navItems } from './navItems'
import { jwtHelper } from '../../../utils/jwtHelper'
import { tokenManager } from '../../../utils/tokenManager'

import { isAdminRole } from '../../../routes/AdminRoute'

import './Sidebar.css'

export default function Sidebar() {
  const token = tokenManager.getToken()
  const role = jwtHelper.getUserRole(token)
  const isAdmin = isAdminRole(role)

  const filteredNavItems = navItems.filter((item) => {
    if (item.adminOnly && !isAdmin) {
      return false
    }
    return true
  })

  return (
    <aside className="sidebar">
      <div className="sidebar__logo">
        MOS
      </div>

      <nav className="sidebar__nav">
        {filteredNavItems.map((item) => (
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