import { createContext, use, useEffect, useState, type ReactNode } from 'react'
import { Navigate } from 'react-router'
import { api } from './api'

type AuthState = {
  /** undefined = still checking, null = signed out */
  email: string | null | undefined
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthState | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [email, setEmail] = useState<string | null | undefined>(undefined)

  // Sign-in is a full-page redirect through Authentik (see LoginPage), so checking
  // the session once on load is enough.
  useEffect(() => {
    api.me().then((me) => setEmail(me.email), () => setEmail(null))
  }, [])

  const logout = async () => {
    await api.logout()
    setEmail(null)
  }

  return <AuthContext value={{ email, logout }}>{children}</AuthContext>
}

export function useAuth() {
  const ctx = use(AuthContext)
  if (!ctx) throw new Error('useAuth doit être utilisé dans <AuthProvider>')
  return ctx
}

/** UI-only guard: the API enforces access with [Authorize]. */
export function RequireAdmin({ children }: { children: ReactNode }) {
  const { email } = useAuth()
  if (email === undefined) return <p>Chargement…</p>
  if (email === null) return <Navigate to="/login" replace />
  return children
}
