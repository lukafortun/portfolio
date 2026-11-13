import type { Localized } from '../../api/content'
import { defaultLanguage, languages } from '../../i18n'
import { Field } from '../ui/Field'
import { Flag } from '../ui/flags'
import styles from './LocalizedField.module.css'

type Props = {
  /** Inputs are named "<name>.fr", "<name>.en"… (read them with readLocalized) */
  name: string
  label: string
  hint?: string
  defaultValue?: Localized
  /** The default language (fr) must be filled in. Other languages are always optional. */
  required?: boolean
  multiline?: boolean
  rows?: number
  maxLength: number
}

/** One translated text: an input per language, each with its flag. */
export function LocalizedField({ name, label, hint, defaultValue, required = false, multiline = false, rows = 3, maxLength }: Props) {
  return (
    <Field as="div" label={label} hint={hint}>
      <div className={styles.rows}>
        {languages.map((language) => {
          const props = {
            name: `${name}.${language}`,
            defaultValue: defaultValue?.[language] ?? '',
            required: required && language === defaultLanguage,
            maxLength,
            lang: language,
            'aria-label': `${label} (${language})`,
          }
          return (
            <div key={language} className={styles.row}>
              <span className={styles.flag} title={language}>
                <Flag language={language} />
              </span>
              {multiline ? <textarea rows={rows} {...props} className={styles.textarea} /> : <input {...props} />}
            </div>
          )
        })}
      </div>
    </Field>
  )
}
