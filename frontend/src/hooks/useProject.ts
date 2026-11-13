import { ApiError } from '../api/client'
import { projectsApi } from '../api/projects'
import { useLanguage } from '../i18n/useLanguage'
import { useFetch } from './useFetch'

/** Loads one project by slug, in the current language. Reloads when the slug or the language changes. */
export function useProject(slug: string) {
  const language = useLanguage()

  const { data, error } = useFetch(async () => {
    try {
      return { slug, project: await projectsApi.getBySlug(slug, language) }
    } catch (e) {
      // A 404 is a normal outcome (unknown slug), not an error to display.
      if (e instanceof ApiError && e.status === 404) return { slug, project: null }
      throw e
    }
  }, `${slug}|${language}`)

  // Ignore a result left over from the previous slug while the new one loads.
  const current = data?.slug === slug ? data : null

  return {
    project: current?.project ?? null,
    notFound: current !== null && current.project === null,
    error,
  }
}
