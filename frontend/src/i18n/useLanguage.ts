import { useTranslation } from 'react-i18next'
import { defaultLanguage, languages, type Language } from '.'

/** The current UI language, narrowed to the supported ones. */
export function useLanguage(): Language {
  const { i18n } = useTranslation()
  const current = i18n.resolvedLanguage
  return languages.find((lng) => lng === current) ?? defaultLanguage
}
