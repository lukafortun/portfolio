import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { projectsApi, type Project, type ProjectInput, type ProjectListItem } from '../api/projects'
import { AdminProjectList } from '../components/admin/AdminProjectList'
import { ProjectForm } from '../components/admin/ProjectForm'
import { Alert } from '../components/ui/Alert'
import { Card } from '../components/ui/Card'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { SplitPane } from '../components/ui/SplitPane'
import { useProjects } from '../hooks/useProjects'
import styles from './AdminPage.module.css'

export function AdminPage() {
  const { t } = useTranslation()
  const { projects, error, reload } = useProjects()
  const [editing, setEditing] = useState<Project | null>(null)
  // Bumped after each save: a new form key remounts the form, which empties it.
  const [formVersion, setFormVersion] = useState(0)
  const [actionError, setActionError] = useState<string | null>(null)

  // The list has no Markdown content: load the full project before editing it.
  const edit = async (item: ProjectListItem) => {
    try {
      setActionError(null)
      setEditing(await projectsApi.get(item.id))
    } catch (e) {
      setActionError((e as Error).message)
    }
  }

  // Errors are thrown back to ProjectForm, which displays them.
  const save = async (input: ProjectInput) => {
    if (editing) await projectsApi.update(editing.id, input)
    else await projectsApi.create(input)
    setEditing(null)
    setFormVersion((v) => v + 1)
    await reload()
  }

  const remove = async (item: ProjectListItem) => {
    if (!confirm(t('admin.confirmDelete', { title: item.title }))) return
    try {
      setActionError(null)
      await projectsApi.remove(item.id)
      if (editing?.id === item.id) setEditing(null)
      await reload()
    } catch (e) {
      setActionError((e as Error).message)
    }
  }

  return (
    <>
      <PageHeader command={t('admin.command')} title={t('admin.title')} subtitle={t('admin.subtitle')} />

      <SplitPane
        label={t('admin.resize')}
        storageKey="admin.formWidth"
        left={
          <Card
            title={editing ? t('admin.editWindow', { title: editing.title }) : t('admin.newWindow')}
            className={styles.formCard}
          >
            <ProjectForm
              key={`${editing?.id ?? 'new'}-${formVersion}`}
              project={editing}
              onSubmit={save}
              onCancel={() => setEditing(null)}
            />
          </Card>
        }
        right={
          <section className={styles.listSection}>
            <h2 className={styles.sectionTitle}>
              <span className={styles.prompt}>$</span> {t('admin.listCommand')}
              <span className={styles.count}>{projects?.length ?? 0}</span>
            </h2>

            {actionError && <Alert>{actionError}</Alert>}

            {error ? (
              <Alert>{error}</Alert>
            ) : !projects ? (
              <Loader />
            ) : projects.length === 0 ? (
              <EmptyState>{t('admin.empty')}</EmptyState>
            ) : (
              <AdminProjectList projects={projects} selectedId={editing?.id} onEdit={edit} onDelete={remove} />
            )}
          </section>
        }
      />
    </>
  )
}
