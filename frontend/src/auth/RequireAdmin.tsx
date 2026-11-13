import type { ReactNode } from 'react'
import { Navigate } from 'react-router'
import { Loader } from '../components/ui/Loader'
import { useAuth } from './useAuth'

/** UI-only guard: the API enforces access with [Authorize]. */
export function RequireAdmin({ children }: { children: ReactNode }) {
  const { email } = useAuth()
  if (email === undefined) return <Loader />
  if (email === null) return <Navigate to="/login" replace />
  return children
}
