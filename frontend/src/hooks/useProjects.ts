import { useCallback, useEffect, useState } from 'react'
import { projectsApi, type Project } from '../api/projects'

/** Loads the project list. `projects` is null while the first request is pending. */
export function useProjects() {
  const [projects, setProjects] = useState<Project[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  const reload = useCallback(
    () =>
      projectsApi.list().then(
        (list) => {
          setProjects(list)
          setError(null)
        },
        (e: Error) => setError(e.message),
      ),
    [],
  )

  // State is only set once the request resolves, not synchronously in the effect.
  useEffect(() => {
    let ignore = false
    projectsApi.list().then(
      (list) => !ignore && setProjects(list),
      (e: Error) => !ignore && setError(e.message),
    )
    return () => {
      ignore = true
    }
  }, [])

  return { projects, error, reload }
}
