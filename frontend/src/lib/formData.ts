import type { Localized } from '../api/content'
import { languages } from '../i18n'

// Helpers to read uncontrolled forms (new FormData(form)).

export function readText(form: FormData, name: string) {
  return String(form.get(name) ?? '').trim()
}

/** Inputs named "role.fr", "role.en"… → { fr: '…', en: '…' } (empty languages left out). */
export function readLocalized(form: FormData, name: string): Localized {
  return Object.fromEntries(
    languages.map((language) => [language, readText(form, `${name}.${language}`)]).filter(([, text]) => text),
  )
}

/** A TagInput's hidden field, "react docker" (or typed "react, docker") → ["react", "docker"]. The API normalizes them. */
export function readTags(form: FormData, name: string) {
  return readText(form, name).split(/[\s,]+/).filter(Boolean)
}

/** "React, TypeScript\nVite" → ["React", "TypeScript", "Vite"] */
export function readList(form: FormData, name: string) {
  return readText(form, name)
    .split(/[,\n]/)
    .map((item) => item.trim())
    .filter(Boolean)
}
