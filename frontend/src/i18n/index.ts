import i18n from 'i18next'
import LanguageDetector from 'i18next-browser-languagedetector'
import { initReactI18next } from 'react-i18next'
import { en } from './locales/en'
import { fr } from './locales/fr'

export const languages = ['fr', 'en'] as const
export type Language = (typeof languages)[number]

/** Fallback UI language. Also the language every project must be written in (see the API). */
export const defaultLanguage: Language = 'fr'

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      fr: { translation: fr },
      en: { translation: en },
    },
    supportedLngs: [...languages],
    fallbackLng: defaultLanguage,
    // "en-US" → "en"
    load: 'languageOnly',
    // React already escapes values, so i18next doesn't need to.
    interpolation: { escapeValue: false },
    detection: {
      // A choice made with the switcher wins, then the browser language.
      order: ['localStorage', 'navigator'],
      lookupLocalStorage: 'lang',
      caches: ['localStorage'],
    },
  })

// Keep <html lang> in sync (screen readers, spell check, hyphenation).
const syncHtmlLang = (lng: string) => {
  document.documentElement.lang = lng
}
syncHtmlLang(i18n.resolvedLanguage ?? defaultLanguage)
i18n.on('languageChanged', syncHtmlLang)

export default i18n
