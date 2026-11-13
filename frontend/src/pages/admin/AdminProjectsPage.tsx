import { useTranslation } from 'react-i18next'
import { projectsApi, type Project, type ProjectListItem } from '../../api/projects'
import { AdminCrud } from '../../components/admin/AdminCrud'
import { AdminItemList } from '../../components/admin/AdminItemList'
import { ProjectForm } from '../../components/admin/ProjectForm'
import { useProjects } from '../../hooks/useProjects'
import { languages } from '../../i18n'
import { formatDate } from '../../lib/format'

export function AdminProjectsPage() {
  const { t } = useTranslation()
  const { projects, error, reload } = useProjects()

  return (
    <AdminCrud<ProjectListItem, Project>
      storageKey="admin.projects.formWidth"
      items={projects}
      error={error}
      reload={reload}
      // The list has no Markdown content: load the full project before editing it.
      load={(item) => projectsApi.get(item.id)}
      remove={projectsApi.remove}
      itemLabel={(p) => p.title}
      windowTitle={(p) => (p ? t('admin.editWindow', { title: p.title }) : t('admin.projects.new'))}
      listCommand={t('admin.projects.list')}
      empty={t('admin.projects.empty')}
      renderForm={({ editing, onSaved, onCancel }) => (
        <ProjectForm
          project={editing}
          onCancel={onCancel}
          onSubmit={async (input) => {
            if (editing) await projectsApi.update(editing.id, input)
            else await projectsApi.create(input)
            await onSaved()
          }}
        />
      )}
      renderList={(props) => (
        <AdminItemList
          {...props}
          columns={t('admin.projects.columns', { returnObjects: true })}
          title={(p) => p.title}
          // Which languages are filled in: [FR][EN]
          badges={(p) => languages.map((lng) => ({ label: lng, on: p.languages.includes(lng) }))}
          meta={(p) => formatDate(p.createdAt)}
        />
      )}
    />
  )
}
