import type { Localized, YearMonth } from '../api/content'
import { defaultLanguage } from '.'
import { useLanguage } from './useLanguage'

/** Helpers to read content written in several languages, in the current one. */
export function useLocalize() {
  const language = useLanguage()
  const monthFormat = new Intl.DateTimeFormat(language, { month: 'short', year: 'numeric' })

  return {
    /** { fr, en } → the text in the current language, or French if it isn't translated. */
    l: (text: Localized | undefined) => text?.[language] || text?.[defaultLanguage] || '',
    /** "2024-09" → "sept. 2024" / "Sep 2024" */
    month: (value: YearMonth) => monthFormat.format(new Date(`${value}-01T00:00:00`)),
  }
}
