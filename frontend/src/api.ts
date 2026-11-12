export type Project = {
  id: number
  title: string
  description: string
  repoUrl: string | null
  liveUrl: string | null
  createdAt: string
}

export type ProjectInput = Omit<Project, 'id' | 'createdAt'>

export class ApiError extends Error {
  readonly status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`/api${path}`, {
    ...init,
    credentials: 'same-origin',
    headers: { 'Content-Type': 'application/json', ...init?.headers },
  })

  if (!response.ok) {
    // Errors are ProblemDetails: { title, detail, errors? }
    const problem = await response.json().catch(() => null)
    throw new ApiError(response.status, problem?.detail ?? problem?.title ?? response.statusText)
  }

  return response.status === 204 ? (undefined as T) : response.json()
}

export const api = {
  /** Navigate to this URL (don't fetch it): the API redirects to Authentik. */
  loginUrl: (returnUrl = '/admin') => `/api/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`,
  logout: () => request<void>('/auth/logout', { method: 'POST' }),
  me: () => request<{ email: string }>('/auth/me'),

  listProjects: () => request<Project[]>('/projects'),
  createProject: (input: ProjectInput) =>
    request<Project>('/projects', { method: 'POST', body: JSON.stringify(input) }),
  updateProject: (id: number, input: ProjectInput) =>
    request<Project>(`/projects/${id}`, { method: 'PUT', body: JSON.stringify(input) }),
  deleteProject: (id: number) => request<void>(`/projects/${id}`, { method: 'DELETE' }),
}
