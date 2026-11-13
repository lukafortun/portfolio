import { request } from './client'

export type Project = {
  id: number
  title: string
  description: string
  repoUrl: string | null
  liveUrl: string | null
  createdAt: string
}

export type ProjectInput = Omit<Project, 'id' | 'createdAt'>

export const projectsApi = {
  list: () => request<Project[]>('/projects'),
  create: (input: ProjectInput) =>
    request<Project>('/projects', { method: 'POST', body: JSON.stringify(input) }),
  update: (id: number, input: ProjectInput) =>
    request<Project>(`/projects/${id}`, { method: 'PUT', body: JSON.stringify(input) }),
  remove: (id: number) => request<void>(`/projects/${id}`, { method: 'DELETE' }),
}
