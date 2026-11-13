import 'i18next'
import type { Translations } from './locales/fr'

// Type-checks translation keys: t('home.title') compiles, t('home.typo') does not.
declare module 'i18next' {
  interface CustomTypeOptions {
    defaultNS: 'translation'
    resources: { translation: Translations }
  }
}
