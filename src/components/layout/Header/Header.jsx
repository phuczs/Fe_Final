import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

import { authApi } from '../../../api/authApi'
import { tokenManager } from '../../../utils/tokenManager'

import MyProfileDrawer from '../../../pages/Home/drawers/MyProfileDrawer'

import './Header.css'

export default function Header() {
  const navigate = useNavigate()

  const [open, setOpen] = useState(false)
  const [isLoggingOut, setIsLoggingOut] =
    useState(false)

  const [isProfileDrawerOpen, setIsProfileDrawerOpen] =
    useState(false)

  const handleLogout = async () => {
    if (isLoggingOut) {
      return
    }

    setIsLoggingOut(true)

    try {
      await authApi.logout()
    } catch {
      // Clear local session even if API call fails
    } finally {
      tokenManager.clearToken()
      sessionStorage.removeItem('tempToken')
      setOpen(false)
      setIsLoggingOut(false)

      navigate('/signin', {
        replace: true,
      })
    }
  }

  return (
    <>
      <header className="header">
        <div className="header__left">
          <h2>Management Portal</h2>
        </div>

        <div className="header__right">
          <button className="header__icon">
            🔔
          </button>

          <button className="header__icon">
            ❔
          </button>

          <div className="header__user">
            <button
              className="user-trigger"
              onClick={() =>
                setOpen((prev) => !prev)
              }
            >
              <div className="avatar">
                N
              </div>

              <div className="user-info">
                <span className="user-name">
                  ncssdev@snapmail.cc
                </span>

                <span className="user-org">
                  NCSSDEV
                </span>
              </div>

              <span>▼</span>
            </button>

            {open && (
              <div className="user-dropdown">
                <button
                  onClick={() => {
                    setIsProfileDrawerOpen(true)
                    setOpen(false)
                  }}
                >
                  Profile
                </button>

                <button>
                  Account Settings
                </button>

                <button>
                  Change Password
                </button>

                <div className="dropdown-divider" />

                <button
                  className="logout-btn"
                  onClick={handleLogout}
                  disabled={isLoggingOut}
                >
                  {isLoggingOut
                    ? 'Logging out...'
                    : 'Logout'}
                </button>
              </div>
            )}
          </div>
        </div>
      </header>

      <MyProfileDrawer
        open={isProfileDrawerOpen}
        onClose={() =>
          setIsProfileDrawerOpen(false)
        }
      />
    </>
  )
}