import { useState, type FormEvent, type ReactNode } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert } from '../ui/Alert'
import { Button } from '../ui/Button'
import styles from './AdminForm.module.css'

type Props = {
  /** Reads the form and saves it. A thrown error is shown under the form. */
  onSubmit: (form: FormData) => Promise<void>
  /** Shows a cancel button (e.g. while editing an existing item) */
  onCancel?: () => void
  submitLabel: string
  /** Message shown after a successful save */
  savedMessage?: string
  children: ReactNode
}

/**
 * Shared shell of the admin forms: submit handling, "sending…" state, error and buttons.
 * Fields are uncontrolled: they are read from FormData on submit.
 */
export function AdminForm({ onSubmit, onCancel, submitLabel, savedMessage, children }: Props) {
  const { t } = useTranslation()
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [saved, setSaved] = useState(false)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSubmitting(true)
    setError(null)
    setSaved(false)
    try {
      await onSubmit(new FormData(event.currentTarget))
      setSaved(true)
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      {children}

      {error && <Alert>{error}</Alert>}

      <div className={styles.actions}>
        {saved && savedMessage && <span className={styles.saved} role="status">{savedMessage}</span>}
        {onCancel && <Button variant="ghost" onClick={onCancel}>{t('form.cancel')}</Button>}
        <Button type="submit" variant="primary" disabled={submitting}>
          {submitting ? t('form.submitting') : submitLabel} ⏎
        </Button>
      </div>
    </form>
  )
}

type SectionProps = {
  legend: ReactNode
  children: ReactNode
  lang?: string
}

/** A group of fields with a legend (one per language, "links"…). */
export function FormSection({ legend, children, lang }: SectionProps) {
  return (
    <fieldset className={styles.section} lang={lang}>
      <legend className={styles.legend}>{legend}</legend>
      {children}
    </fieldset>
  )
}

/** "required" / "optional" badge, for a section legend. */
export function RequiredBadge({ required }: { required: boolean }) {
  const { t } = useTranslation()
  return (
    <span className={required ? styles.required : styles.optional}>
      {required ? t('form.required') : t('form.optional')}
    </span>
  )
}
