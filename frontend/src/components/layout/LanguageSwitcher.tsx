import { useTranslation } from 'react-i18next'
import { languages } from '../../i18n'
import { useLanguage } from '../../i18n/useLanguage'
import { cx } from '../../lib/cx'
import { Flag } from '../ui/flags'
import styles from './LanguageSwitcher.module.css'

export function LanguageSwitcher() {
  const { t, i18n } = useTranslation()
  const current = useLanguage()

  return (
    <div className={styles.switcher} role="group" aria-label={t('language.label')}>
      {languages.map((lng) => (
        <button
          key={lng}
          type="button"
          lang={lng}
          className={cx(styles.option, lng === current && styles.active)}
          aria-pressed={lng === current}
          title={t(`language.${lng}`)}
          onClick={() => i18n.changeLanguage(lng)}
        >
          <Flag language={lng} />
          <span className={styles.code}>{lng}</span>
        </button>
      ))}
    </div>
  )
}
