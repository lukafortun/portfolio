import { useEffect, useState, type ReactNode } from 'react'
import { authApi } from '../api/auth'
import { AuthContext } from './AuthContext'

export function AuthProvider({ children }: { children: ReactNode }) {
  const [email, setEmail] = useState<string | null | undefined>(undefined)

  // Sign-in is a full-page redirect through Authentik, so checking the session once on load is enough.
  useEffect(() => {
    authApi.me().then((me) => setEmail(me.email), () => setEmail(null))
  }, [])

  const logout = async () => {
    await authApi.logout()
    setEmail(null)
  }

  return <AuthContext value={{ email, logout }}>{children}</AuthContext>
}
