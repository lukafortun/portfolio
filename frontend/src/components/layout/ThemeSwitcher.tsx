import { useEffect, useState, type ChangeEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { applyTheme, readChoice, resolveTheme, saveChoice, themes, watchSystemTheme, type ThemeChoice } from '../../theme/theme'
import styles from './ThemeSwitcher.module.css'

/** Palette picker in the header. The choice is saved in localStorage, per visitor. */
export function ThemeSwitcher() {
  const { t } = useTranslation()
  const [choice, setChoice] = useState(readChoice)

  useEffect(() => {
    applyTheme(resolveTheme(choice))
    // In auto mode, follow the system when it switches between light and dark.
    if (choice === 'auto') return watchSystemTheme(() => applyTheme(resolveTheme('auto')))
  }, [choice])

  const onChange = (e: ChangeEvent<HTMLSelectElement>) => {
    const next = e.target.value as ThemeChoice
    saveChoice(next)
    setChoice(next)
  }

  return (
    <label className={styles.switcher} title={t('theme.label')}>
      {/* Shows the current accent color. */}
      <span className={styles.swatch} aria-hidden />
      <select className={styles.select} value={choice} onChange={onChange} aria-label={t('theme.label')}>
        <option value="auto">{t('theme.auto')}</option>
        {themes.map((theme) => (
          <option key={theme} value={theme}>
            {theme}
          </option>
        ))}
      </select>
    </label>
  )
}
