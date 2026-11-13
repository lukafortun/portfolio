import { useTranslation } from 'react-i18next'
import type { Experience, ExperienceInput } from '../../api/content'
import { readList, readLocalized, readText } from '../../lib/formData'
import { Field } from '../ui/Field'
import { AdminForm } from './AdminForm'
import { LocalizedField } from './LocalizedField'
import styles from './ExperienceForm.module.css'

type Props = {
  experience: Experience | null
  onSubmit: (input: ExperienceInput) => Promise<void>
  onCancel: () => void
}

// <input type="month"> uses the same "YYYY-MM" format as the API.
function readInput(form: FormData): ExperienceInput {
  return {
    start: readText(form, 'start'),
    end: readText(form, 'end') || null,
    company: readText(form, 'company'),
    role: readLocalized(form, 'role'),
    location: readLocalized(form, 'location'),
    description: readLocalized(form, 'description'),
    tags: readList(form, 'tags'),
  }
}

export function ExperienceForm({ experience, onSubmit, onCancel }: Props) {
  const { t } = useTranslation()

  return (
    <AdminForm
      onSubmit={(form) => onSubmit(readInput(form))}
      onCancel={experience ? onCancel : undefined}
      submitLabel={experience ? t('form.save') : t('form.create')}
    >
      <div className={styles.dates}>
        <Field label={t('form.start')}>
          <input name="start" type="month" defaultValue={experience?.start} required />
        </Field>
        <Field label={t('form.end')} hint={t('form.endHint')}>
          <input name="end" type="month" defaultValue={experience?.end ?? ''} />
        </Field>
      </div>
      <Field label={t('form.company')}>
        <input name="company" defaultValue={experience?.company} required maxLength={100} />
      </Field>
      <LocalizedField name="role" label={t('form.role')} defaultValue={experience?.role} required maxLength={100} />
      <LocalizedField name="location" label={t('form.location')} defaultValue={experience?.location} maxLength={100} />
      <LocalizedField
        name="description"
        label={t('form.description')}
        defaultValue={experience?.description}
        multiline
        rows={4}
        maxLength={2000}
      />
      <Field label={t('form.tags')} hint={t('form.tagsHint')}>
        <input name="tags" defaultValue={experience?.tags.join(', ')} placeholder="React, C#, Docker" />
      </Field>
    </AdminForm>
  )
}
