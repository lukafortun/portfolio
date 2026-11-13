import { useState, type FormEvent } from 'react'
import type { Project, ProjectInput } from '../../api/projects'
import { Alert } from '../ui/Alert'
import { Button } from '../ui/Button'
import { Field } from '../ui/Field'
import styles from './ProjectForm.module.css'

type Props = {
  /** The project being edited, or null to create a new one */
  project: Project | null
  onSubmit: (input: ProjectInput) => Promise<void>
  onCancel: () => void
}

function readInput(form: FormData): ProjectInput {
  const text = (name: string) => String(form.get(name) ?? '').trim()
  return {
    title: text('title'),
    description: text('description'),
    repoUrl: text('repoUrl') || null,
    liveUrl: text('liveUrl') || null,
  }
}

/**
 * Uncontrolled form: inputs keep their own value and are read on submit.
 * The parent gives it a `key` per project, so switching project remounts it with fresh defaultValues.
 */
export function ProjectForm({ project, onSubmit, onCancel }: Props) {
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const form = event.currentTarget
    setSubmitting(true)
    setError(null)
    try {
      await onSubmit(readInput(new FormData(form)))
      form.reset()
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <Field label="titre">
        <input name="title" defaultValue={project?.title} required maxLength={200} />
      </Field>
      <Field label="description">
        <textarea name="description" defaultValue={project?.description} required maxLength={4000} />
      </Field>
      <Field label="repo" hint="Lien vers le code source (optionnel)">
        <input name="repoUrl" type="url" placeholder="https://github.com/…" defaultValue={project?.repoUrl ?? ''} />
      </Field>
      <Field label="demo" hint="Lien vers la version en ligne (optionnel)">
        <input name="liveUrl" type="url" placeholder="https://…" defaultValue={project?.liveUrl ?? ''} />
      </Field>

      {error && <Alert>{error}</Alert>}

      <div className={styles.actions}>
        {project && <Button variant="ghost" onClick={onCancel}>annuler</Button>}
        <Button type="submit" variant="primary" disabled={submitting}>
          {submitting ? 'envoi…' : project ? 'enregistrer' : 'créer'} ⏎
        </Button>
      </div>
    </form>
  )
}
