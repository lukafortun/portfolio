import { createContext } from 'react'

export type AuthState = {
  /** undefined = still checking, null = signed out */
  email: string | null | undefined
  logout: () => Promise<void>
}

export const AuthContext = createContext<AuthState | null>(null)
