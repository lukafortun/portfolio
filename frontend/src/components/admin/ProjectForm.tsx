import { useTranslation } from 'react-i18next'
import type { Project, ProjectInput } from '../../api/projects'
import { defaultLanguage, languages } from '../../i18n'
import { readTags, readText } from '../../lib/formData'
import { slugify } from '../../lib/slugify'
import { Field } from '../ui/Field'
import { Flag } from '../ui/flags'
import { AdminForm, FormSection, RequiredBadge } from './AdminForm'
import { CoverImageField } from './CoverImageField'
import { MarkdownEditor } from './MarkdownEditor'
import styles from './ProjectForm.module.css'
import { TagInput } from './TagInput'

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
  const translations = languages
    .map((language) => ({
      language,
      title: readText(form, `title.${language}`),
      summary: readText(form, `summary.${language}`),
      content: readText(form, `content.${language}`),
    }))
    // Optional languages left empty are not sent.
    .filter((t) => t.title || t.summary || t.content)

  return {
    slug: readText(form, 'slug') || slugify(readText(form, `title.${defaultLanguage}`)),
    translations,
    repoUrl: readText(form, 'repoUrl') || null,
    liveUrl: readText(form, 'liveUrl') || null,
    coverImage: readText(form, 'coverImage') || null,
    tags: readTags(form, 'tags'),
  }
}

/** Project form: a section per language (title, summary, Markdown content), then cover, slug and links. */
export function ProjectForm({ project, onSubmit, onCancel }: Props) {
  const { t } = useTranslation()

  return (
    <AdminForm
      onSubmit={(form) => onSubmit(readInput(form))}
      onCancel={project ? onCancel : undefined}
      submitLabel={project ? t('form.save') : t('form.create')}
    >
      {languages.map((language) => {
        const translation = project?.translations.find((tr) => tr.language === language)
        const required = language === defaultLanguage

        return (
          <FormSection
            key={language}
            lang={language}
            legend={
              <>
                <Flag language={language} />
                {t(`language.${language}`)}
                <RequiredBadge required={required} />
              </>
            }
          >
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
          </FormSection>
        )
      })}

      <FormSection legend={t('form.meta')}>
        <Field as="div" label={t('form.cover')} hint={t('form.coverHint')}>
          <CoverImageField name="coverImage" defaultValue={project?.coverImage} />
        </Field>
        {/* as="div": the field holds buttons (remove a tag), a <label> wrapper would also trigger them. */}
        <Field as="div" label={t('form.tags')} hint={t('form.tagsHint', { max: 15 })}>
          <TagInput name="tags" label={t('form.tags')} defaultValue={project?.tags} />
        </Field>
        <Field label={t('form.slug')} hint={t('form.slugHint')}>
          <input name="slug" defaultValue={project?.slug} pattern={slugPattern} maxLength={100} placeholder="mon-projet" />
        </Field>
        <Field label={t('form.repo')} hint={t('form.repoHint')}>
          <input name="repoUrl" type="url" placeholder="https://github.com/…" defaultValue={project?.repoUrl ?? ''} />
        </Field>
        <Field label={t('form.demo')} hint={t('form.demoHint')}>
          <input name="liveUrl" type="url" placeholder="https://…" defaultValue={project?.liveUrl ?? ''} />
        </Field>
      </FormSection>
    </AdminForm>
  )
}
