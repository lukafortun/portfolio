import { request } from './client'

export const authApi = {
  /** Navigate to this URL (don't fetch it): the API redirects to Authentik. */
  loginUrl: (returnUrl = '/admin') => `/api/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`,
  logout: () => request<void>('/auth/logout', { method: 'POST' }),
  me: () => request<{ email: string }>('/auth/me'),
}
