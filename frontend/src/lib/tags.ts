/** URL of the project list filtered by a tag. */
export const tagUrl = (tag: string) => `/projects?tag=${encodeURIComponent(tag)}`

/**
 * Same rule as the API (ProjectMappings.NormalizeTags): "#Spring Boot" → "spring-boot".
 * Applied while typing, so the admin sees the tag as it will be saved.
 */
export function normalizeTag(raw: string) {
  return raw.trim().replace(/^#+/, '').toLowerCase().split(/\s+/).filter(Boolean).join('-')
}
