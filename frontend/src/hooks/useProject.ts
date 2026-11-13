import { useEffect, useState } from 'react'
import { projectsApi, type Project } from '../api/projects'
import { useLanguage } from '../i18n/useLanguage'

/** Loads one project by slug, in the current language. Reloads when the slug or the language changes. */
export function useProject(slug: string) {
  const language = useLanguage()
  const [project, setProject] = useState<Project | null>(null)
  const [error, setError] = useState<Error | null>(null)

  useEffect(() => {
    let ignore = false
    projectsApi.getBySlug(slug, language).then(
      (p) => {
        if (ignore) return
        setProject(p)
        setError(null)
      },
      (e: Error) => !ignore && setError(e),
    )
    return () => {
      ignore = true
    }
  }, [slug, language])

  // Ignore a project left over from the previous slug while the new one loads.
  return { project: project?.slug === slug ? project : null, error }
}
