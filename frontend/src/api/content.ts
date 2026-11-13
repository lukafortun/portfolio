import type { Language } from '../i18n'
import { request } from './client'

/** A text in several languages: { fr: '…', en: '…' }. Only "fr" is guaranteed. */
export type Localized = Partial<Record<Language, string>>

/** "2024-09" (year-month) */
export type YearMonth = string

// ---------- Profile ----------

export type ProfileLink = { label: string; url: string }

export type Profile = {
  name: string
  role: Localized
  bio: Localized
  location: Localized
  /** Contact address, shown as a mailto link; null when not set. */
  email: string | null
  links: ProfileLink[]
}

export const profileApi = {
  get: () => request<Profile>('/profile'),
  save: (input: Profile) => request<Profile>('/profile', { method: 'PUT', body: JSON.stringify(input) }),
}

// ---------- Skills ----------

export type SkillGroup = {
  id: number
  /** Shown as a folder name, e.g. "frontend" */
  name: string
  /** Display order, ascending */
  sortOrder: number
  description: Localized
  skills: string[]
}

export type SkillGroupInput = Omit<SkillGroup, 'id'>

export const skillsApi = {
  list: () => request<SkillGroup[]>('/skills'),
  create: (input: SkillGroupInput) => request<SkillGroup>('/skills', { method: 'POST', body: JSON.stringify(input) }),
  update: (id: number, input: SkillGroupInput) =>
    request<SkillGroup>(`/skills/${id}`, { method: 'PUT', body: JSON.stringify(input) }),
  remove: (id: number) => request<void>(`/skills/${id}`, { method: 'DELETE' }),
}

// ---------- Experience ----------

export type Experience = {
  id: number
  start: YearMonth
  /** null = ongoing */
  end: YearMonth | null
  company: string
  role: Localized
  location: Localized
  description: Localized
  tags: string[]
}

export type ExperienceInput = Omit<Experience, 'id'>

export const experiencesApi = {
  /** Most recent first (ongoing entries on top). */
  list: () => request<Experience[]>('/experiences'),
  create: (input: ExperienceInput) =>
    request<Experience>('/experiences', { method: 'POST', body: JSON.stringify(input) }),
  update: (id: number, input: ExperienceInput) =>
    request<Experience>(`/experiences/${id}`, { method: 'PUT', body: JSON.stringify(input) }),
  remove: (id: number) => request<void>(`/experiences/${id}`, { method: 'DELETE' }),
}
