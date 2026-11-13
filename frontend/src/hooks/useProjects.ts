import { useCallback, useEffect, useState } from 'react'
import { projectsApi, type ProjectListItem } from '../api/projects'
import { useLanguage } from '../i18n/useLanguage'

/**
 * Loads the project list in the current language, and reloads it when the language changes.
 * `projects` is null while the first request is pending.
 */
export function useProjects() {
  const language = useLanguage()
  const [projects, setProjects] = useState<ProjectListItem[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  const reload = useCallback(
    () =>
      projectsApi.list(language).then(
        (list) => {
          setProjects(list)
          setError(null)
        },
        (e: Error) => setError(e.message),
      ),
    [language],
  )

  // State is only set once the request resolves, not synchronously in the effect.
  useEffect(() => {
    let ignore = false
    projectsApi.list(language).then(
      (list) => !ignore && setProjects(list),
      (e: Error) => !ignore && setError(e.message),
    )
    return () => {
      ignore = true
    }
  }, [language])

  return { projects, error, reload }
}
