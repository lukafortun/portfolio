import { projectsApi } from '../api/projects'
import { useLanguage } from '../i18n/useLanguage'
import { useFetch } from './useFetch'

/** The project list in the current language; reloaded when the language changes. */
export function useProjects() {
  const language = useLanguage()
  const { data, error, reload } = useFetch(() => projectsApi.list(language), language)
  return { projects: data, error, reload }
}
