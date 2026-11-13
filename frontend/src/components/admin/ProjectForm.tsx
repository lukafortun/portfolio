import { useState, type FormEvent } from 'react'
import { useTranslation } from 'react-i18next'
import type { Project, ProjectInput } from '../../api/projects'
import { defaultLanguage, languages } from '../../i18n'
import { slugify } from '../../lib/slugify'
import { Alert } from '../ui/Alert'
import { Button } from '../ui/Button'
import { Field } from '../ui/Field'
import { Flag } from '../ui/flags'
import { CoverImageField } from './CoverImageField'
import { MarkdownEditor } from './MarkdownEditor'
import styles from './ProjectForm.module.css'

type Props = {
  /** The project being edited, or null to create a new one */
  project: Project | null
  onSubmit: (input: ProjectInput) => Promise<void>
  onCancel: () => void
}

// Same rule as the API (ProjectRequest.SlugPattern).
const slugPattern = '[a-z0-9]+(-[a-z0-9]+)*'

// Inputs are named "title.fr", "content.en"…
function readInput(form: FormData): ProjectInput {
  const text = (name: string) => String(form.get(name) ?? '').trim()

  const translations = languages
    .map((language) => ({
      language,
      title: text(`title.${language}`),
      summary: text(`summary.${language}`),
      content: text(`content.${language}`),
    }))
    // Optional languages left empty are not sent.
    .filter((t) => t.title || t.summary || t.content)

  return {
    slug: text('slug') || slugify(text(`title.${defaultLanguage}`)),
    translations,
    repoUrl: text('repoUrl') || null,
    liveUrl: text('liveUrl') || null,
    coverImage: text('coverImage') || null,
  }
}

/**
 * Uncontrolled form: inputs keep their own value and are read on submit.
 * The parent gives it a `key` per project, so switching project remounts it with fresh defaultValues.
 */
export function ProjectForm({ project, onSubmit, onCancel }: Props) {
  const { t } = useTranslation()
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const form = event.currentTarget
    setSubmitting(true)
    setError(null)
    try {
      // On success the parent remounts this form (new key), which clears it.
      await onSubmit(readInput(new FormData(form)))
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      {languages.map((language) => {
        const translation = project?.translations.find((tr) => tr.language === language)
        const required = language === defaultLanguage

        return (
          <fieldset key={language} className={styles.section} lang={language}>
            <legend className={styles.legend}>
              <Flag language={language} />
              {t(`language.${language}`)}
              <span className={required ? styles.required : styles.optional}>
                {required ? t('form.required') : t('form.optional')}
              </span>
            </legend>
            <Field label={t('form.title')}>
              <input name={`title.${language}`} defaultValue={translation?.title} required={required} maxLength={200} />
            </Field>
            <Field label={t('form.summary')} hint={t('form.summaryHint')}>
              <textarea
                name={`summary.${language}`}
                defaultValue={translation?.summary}
                required={required}
                maxLength={500}
                rows={3}
                className={styles.summary}
              />
            </Field>
            <Field as="div" label={t('form.content')} hint={t('form.contentHint')}>
              <MarkdownEditor
                name={`content.${language}`}
                label={`${t('form.content')} (${language})`}
                defaultValue={translation?.content}
                maxLength={100_000}
              />
            </Field>
          </fieldset>
        )
      })}

      <fieldset className={styles.section}>
        <legend className={styles.legend}>{t('form.meta')}</legend>
        <Field as="div" label={t('form.cover')} hint={t('form.coverHint')}>
          <CoverImageField name="coverImage" defaultValue={project?.coverImage} />
        </Field>
        <Field label={t('form.slug')} hint={t('form.slugHint')}>
          <input
            name="slug"
            defaultValue={project?.slug}
            pattern={slugPattern}
            maxLength={100}
            placeholder="mon-projet"
          />
        </Field>
        <Field label={t('form.repo')} hint={t('form.repoHint')}>
          <input name="repoUrl" type="url" placeholder="https://github.com/…" defaultValue={project?.repoUrl ?? ''} />
        </Field>
        <Field label={t('form.demo')} hint={t('form.demoHint')}>
          <input name="liveUrl" type="url" placeholder="https://…" defaultValue={project?.liveUrl ?? ''} />
        </Field>
      </fieldset>

      {error && <Alert>{error}</Alert>}

      <div className={styles.actions}>
        {project && <Button variant="ghost" onClick={onCancel}>{t('form.cancel')}</Button>}
        <Button type="submit" variant="primary" disabled={submitting}>
          {submitting ? t('form.submitting') : project ? t('form.save') : t('form.create')} ⏎
        </Button>
      </div>
    </form>
  )
}
