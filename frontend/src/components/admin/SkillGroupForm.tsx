import { useTranslation } from 'react-i18next'
import type { SkillGroup, SkillGroupInput } from '../../api/content'
import { readList, readLocalized, readText } from '../../lib/formData'
import { Field } from '../ui/Field'
import { AdminForm } from './AdminForm'
import { LocalizedField } from './LocalizedField'

type Props = {
  group: SkillGroup | null
  onSubmit: (input: SkillGroupInput) => Promise<void>
  onCancel: () => void
}

function readInput(form: FormData): SkillGroupInput {
  return {
    name: readText(form, 'name'),
    sortOrder: Number(readText(form, 'sortOrder')) || 0,
    description: readLocalized(form, 'description'),
    skills: readList(form, 'skills'),
  }
}

export function SkillGroupForm({ group, onSubmit, onCancel }: Props) {
  const { t } = useTranslation()

  return (
    <AdminForm
      onSubmit={(form) => onSubmit(readInput(form))}
      onCancel={group ? onCancel : undefined}
      submitLabel={group ? t('form.save') : t('form.create')}
    >
      <Field label={t('form.groupName')} hint={t('form.groupNameHint')}>
        <input name="name" defaultValue={group?.name} required maxLength={50} placeholder="frontend" />
      </Field>
      <Field label={t('form.sortOrder')} hint={t('form.sortOrderHint')}>
        <input name="sortOrder" type="number" min={0} max={1000} step={1} defaultValue={group?.sortOrder ?? 100} />
      </Field>
      <LocalizedField
        name="description"
        label={t('form.description')}
        defaultValue={group?.description}
        multiline
        rows={2}
        maxLength={300}
      />
      <Field label={t('form.skills')} hint={t('form.skillsHint')}>
        <textarea name="skills" defaultValue={group?.skills.join(', ')} required rows={3} placeholder="React, TypeScript, Vite" />
      </Field>
    </AdminForm>
  )
}
