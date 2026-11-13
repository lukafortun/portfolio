/** Shown in the browser tab before the profile loads, or when it has no name (same as <title> in index.html). */
const fallback = '~/portfolio'

/** The browser tab title: the name set in the admin profile, e.g. "Luka Fortun". */
export function setSiteTitle(name: string | null | undefined) {
  document.title = name?.trim() || fallback
}
