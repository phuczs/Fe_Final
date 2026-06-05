import { useEffect, useState } from 'react'

const SESSION_KEY = 'frontend-final-assessment-session'
const SESSION_EVENT = 'frontend-final-assessment-session-changed'

function getSessionValue() {
  return window.localStorage.getItem(SESSION_KEY) === 'true'
}

export function useAuthSession() {
  const [isAuthenticated, setIsAuthenticated] = useState(() => {
    return getSessionValue()
  })

  useEffect(() => {
    const handleSessionChange = () => {
      setIsAuthenticated(getSessionValue())
    }

    window.addEventListener(SESSION_EVENT, handleSessionChange)

    return () => {
      window.removeEventListener(SESSION_EVENT, handleSessionChange)
    }
  }, [])

  useEffect(() => {
    window.localStorage.setItem(SESSION_KEY, String(isAuthenticated))
    window.dispatchEvent(new Event(SESSION_EVENT))
  }, [isAuthenticated])

  const signIn = () => {
    setIsAuthenticated(true)
  }

  const signOut = () => {
    setIsAuthenticated(false)
  }

  return {
    isAuthenticated,
    signIn,
    signOut,
  }
}