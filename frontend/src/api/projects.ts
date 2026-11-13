import type { Language } from '../i18n'
import { request } from './client'

export type ProjectTranslation = {
  language: Language
  title: string
  /** Short plain text, shown on the cards */
  summary: string
  /** Project page, in Markdown */
  content: string
}

/** A project in the list (no Markdown content). */
export type ProjectListItem = {
  id: number
  slug: string
  /** Language of title/summary: the requested one, or "fr" if the project has no translation for it. */
  language: Language
  title: string
  summary: string
  repoUrl: string | null
  liveUrl: string | null
  /** Uploaded image ("/api/uploads/…") or external URL */
  coverImage: string | null
  createdAt: string
  /** Languages this project is translated into */
  languages: Language[]
}

/** A full project, for its page and for the admin form. */
export type Project = Omit<ProjectListItem, 'languages'> & {
  content: string
  translations: ProjectTranslation[]
}

/** Body of POST/PUT. A "fr" translation is required, other languages are optional. */
export type ProjectInput = {
  slug: string
  translations: ProjectTranslation[]
  repoUrl: string | null
  liveUrl: string | null
  coverImage: string | null
}

export const projectsApi = {
  list: (lang: Language) => request<ProjectListItem[]>(`/projects?lang=${lang}`),
  get: (id: number) => request<Project>(`/projects/${id}`),
  getBySlug: (slug: string, lang: Language) =>
    request<Project>(`/projects/by-slug/${encodeURIComponent(slug)}?lang=${lang}`),
  create: (input: ProjectInput) =>
    request<Project>('/projects', { method: 'POST', body: JSON.stringify(input) }),
  update: (id: number, input: ProjectInput) =>
    request<Project>(`/projects/${id}`, { method: 'PUT', body: JSON.stringify(input) }),
  remove: (id: number) => request<void>(`/projects/${id}`, { method: 'DELETE' }),
}
