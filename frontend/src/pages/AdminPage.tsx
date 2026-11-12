import { useEffect, useState, type FormEvent } from 'react'
import { api, type Project, type ProjectInput } from '../api'

const empty: ProjectInput = { title: '', description: '', repoUrl: null, liveUrl: null }

export function AdminPage() {
  const [projects, setProjects] = useState<Project[]>([])
  const [editing, setEditing] = useState<Project | null>(null)
  const [error, setError] = useState<string | null>(null)

  const reload = () => api.listProjects().then(setProjects, (e: Error) => setError(e.message))
  useEffect(() => { reload() }, [])

  const onSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const formElement = event.currentTarget
    const form = new FormData(formElement)
    const input: ProjectInput = {
      title: String(form.get('title')),
      description: String(form.get('description')),
      repoUrl: String(form.get('repoUrl')) || null,
      liveUrl: String(form.get('liveUrl')) || null,
    }
    try {
      setError(null)
      if (editing) await api.updateProject(editing.id, input)
      else await api.createProject(input)
      setEditing(null)
      formElement.reset()
      await reload()
    } catch (e) {
      setError((e as Error).message)
    }
  }

  const onDelete = async (p: Project) => {
    if (!confirm(`Supprimer « ${p.title} » ?`)) return
    await api.deleteProject(p.id)
    await reload()
  }

  const current = editing ?? empty

  return (
    <section>
      <h1>Administration</h1>

      {/* Changing the key remounts the form, resetting its defaultValues for the edited project. */}
      <form key={editing?.id ?? 'new'} onSubmit={onSubmit} className="form">
        <h2>{editing ? `Modifier « ${editing.title} »` : 'Nouveau projet'}</h2>
        <label>Titre<input name="title" defaultValue={current.title} required maxLength={200} /></label>
        <label>Description<textarea name="description" defaultValue={current.description} required rows={4} /></label>
        <label>URL du code<input name="repoUrl" type="url" defaultValue={current.repoUrl ?? ''} /></label>
        <label>URL de la démo<input name="liveUrl" type="url" defaultValue={current.liveUrl ?? ''} /></label>
        {error && <p className="error">{error}</p>}
        <div className="row">
          <button type="submit">{editing ? 'Enregistrer' : 'Créer'}</button>
          {editing && <button type="button" onClick={() => setEditing(null)}>Annuler</button>}
        </div>
      </form>

      <table className="table">
        <thead><tr><th>Titre</th><th>Créé le</th><th /></tr></thead>
        <tbody>
          {projects.map((p) => (
            <tr key={p.id}>
              <td>{p.title}</td>
              <td>{new Date(p.createdAt).toLocaleDateString('fr-FR')}</td>
              <td className="row">
                <button onClick={() => setEditing(p)}>Modifier</button>
                <button onClick={() => onDelete(p)}>Supprimer</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  )
}
