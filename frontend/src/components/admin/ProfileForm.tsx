import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import type { Profile, ProfileLink } from '../../api/content'
import { readLocalized, readText } from '../../lib/formData'
import { Button } from '../ui/Button'
import { Field } from '../ui/Field'
import { AdminForm, FormSection } from './AdminForm'
import { LocalizedField } from './LocalizedField'
import styles from './ProfileForm.module.css'

type Props = {
  profile: Profile
  onSubmit: (input: Profile) => Promise<void>
}

// Every link row has inputs named "linkLabel" and "linkUrl": getAll() returns them in order.
function readInput(form: FormData): Profile {
  const labels = form.getAll('linkLabel').map((v) => String(v).trim())
  const urls = form.getAll('linkUrl').map((v) => String(v).trim())

  return {
    name: readText(form, 'name'),
    role: readLocalized(form, 'role'),
    location: readLocalized(form, 'location'),
    bio: readLocalized(form, 'bio'),
    email: readText(form, 'email') || null,
    links: labels.map((label, i) => ({ label, url: urls[i] })).filter((link) => link.label && link.url),
  }
}

export function ProfileForm({ profile, onSubmit }: Props) {
  const { t } = useTranslation()

  return (
    <AdminForm onSubmit={(form) => onSubmit(readInput(form))} submitLabel={t('form.save')} savedMessage={t('admin.saved')}>
      <Field label={t('form.name')}>
        <input name="name" defaultValue={profile.name} required maxLength={100} />
      </Field>
      <LocalizedField name="role" label={t('form.role')} defaultValue={profile.role} required maxLength={100} />
      <LocalizedField name="location" label={t('form.location')} defaultValue={profile.location} maxLength={100} />
      <LocalizedField
        name="bio"
        label={t('form.bio')}
        hint={t('form.bioHint')}
        defaultValue={profile.bio}
        required
        multiline
        rows={5}
        maxLength={2000}
      />
      <Field label={t('form.email')} hint={t('form.emailHint')}>
        <input name="email" type="email" defaultValue={profile.email ?? ''} maxLength={200} placeholder="prenom@exemple.com" />
      </Field>
      <FormSection legend={t('form.links')}>
        <LinksEditor defaultValue={profile.links} />
      </FormSection>
    </AdminForm>
  )
}

/**
 * Rows of (label, url) inputs. Only the list of rows lives in state (to add/remove some);
 * the values themselves stay in the inputs and are read with FormData.
 */
function LinksEditor({ defaultValue }: { defaultValue: ProfileLink[] }) {
  const { t } = useTranslation()
  // A stable id per row, so React keeps each row's input values when another row is removed.
  const [rows, setRows] = useState(() => defaultValue.map((link, i) => ({ id: i, ...link })))
  const [nextId, setNextId] = useState(defaultValue.length)

  const add = () => {
    setRows((current) => [...current, { id: nextId, label: '', url: '' }])
    setNextId((id) => id + 1)
  }

  return (
    <div className={styles.links}>
      {rows.map((row) => (
        <div key={row.id} className={styles.link}>
          <input name="linkLabel" defaultValue={row.label} placeholder="github" maxLength={50} aria-label={t('form.linkLabel')} />
          <input name="linkUrl" type="url" defaultValue={row.url} placeholder="https://…" maxLength={500} aria-label={t('form.linkUrl')} />
          <Button
            variant="danger"
            size="sm"
            onClick={() => setRows((current) => current.filter((r) => r.id !== row.id))}
            aria-label={t('form.removeLink')}
            title={t('form.removeLink')}
          >
            ×
          </Button>
        </div>
      ))}
      <Button size="sm" onClick={add} className={styles.add}>
        {t('form.addLink')}
      </Button>
    </div>
  )
}
