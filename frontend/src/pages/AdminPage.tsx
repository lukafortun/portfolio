import { useState } from 'react'
import { projectsApi, type Project, type ProjectInput } from '../api/projects'
import { AdminProjectList } from '../components/admin/AdminProjectList'
import { ProjectForm } from '../components/admin/ProjectForm'
import { Alert } from '../components/ui/Alert'
import { Card } from '../components/ui/Card'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useProjects } from '../hooks/useProjects'
import styles from './AdminPage.module.css'

export function AdminPage() {
  const { projects, error, reload } = useProjects()
  const [editing, setEditing] = useState<Project | null>(null)
  const [deleteError, setDeleteError] = useState<string | null>(null)

  // Errors are thrown back to ProjectForm, which displays them.
  const save = async (input: ProjectInput) => {
    if (editing) await projectsApi.update(editing.id, input)
    else await projectsApi.create(input)
    setEditing(null)
    await reload()
  }

  const remove = async (project: Project) => {
    if (!confirm(`Supprimer « ${project.title} » ?`)) return
    try {
      setDeleteError(null)
      await projectsApi.remove(project.id)
      if (editing?.id === project.id) setEditing(null)
      await reload()
    } catch (e) {
      setDeleteError((e as Error).message)
    }
  }

  return (
    <>
      <PageHeader
        command="sudo admin"
        title="Gérer les projets"
        subtitle="Ajoute, modifie ou supprime les projets affichés sur le site."
      />

      <div className={styles.layout}>
        <Card title={editing ? `edit — ${editing.title}` : 'new-project'} className={styles.formCard}>
          <ProjectForm key={editing?.id ?? 'new'} project={editing} onSubmit={save} onCancel={() => setEditing(null)} />
        </Card>

        <section className={styles.listSection}>
          <h2 className={styles.sectionTitle}>
            <span className={styles.prompt}>$</span> ls -l projets
            <span className={styles.count}>{projects?.length ?? 0}</span>
          </h2>

          {deleteError && <Alert>{deleteError}</Alert>}

          {error ? (
            <Alert>{error}</Alert>
          ) : !projects ? (
            <Loader />
          ) : projects.length === 0 ? (
            <EmptyState>aucun projet, crée le premier avec le formulaire</EmptyState>
          ) : (
            <AdminProjectList projects={projects} selectedId={editing?.id} onEdit={setEditing} onDelete={remove} />
          )}
        </section>
      </div>
    </>
  )
}
