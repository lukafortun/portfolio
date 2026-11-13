export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

/** Calls the API (same origin, so the auth cookie is sent) and parses the JSON response. */
export async function request<T>(path: string, init?: RequestInit): Promise<T> {
  // FormData (file uploads) sets its own multipart Content-Type, with the boundary.
  const json = typeof init?.body === 'string'
  const response = await fetch(`/api${path}`, {
    ...init,
    credentials: 'same-origin',
    headers: { ...(json && { 'Content-Type': 'application/json' }), ...init?.headers },
  })

  if (!response.ok) {
    // Errors are ProblemDetails: { title, detail, errors? }. For a 400, show the first validation error.
    const problem = await response.json().catch(() => null)
    const firstError = problem?.errors ? Object.values<string[]>(problem.errors).flat()[0] : undefined
    throw new ApiError(response.status, firstError ?? problem?.detail ?? problem?.title ?? response.statusText)
  }

  return response.status === 204 ? (undefined as T) : response.json()
}
