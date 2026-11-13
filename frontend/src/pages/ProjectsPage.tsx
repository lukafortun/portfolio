import { useTranslation } from 'react-i18next'
import { Link, useSearchParams } from 'react-router'
import { ProjectGrid } from '../components/projects/ProjectGrid'
import { TagFilter } from '../components/projects/TagFilter'
import { Alert } from '../components/ui/Alert'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useProjects } from '../hooks/useProjects'
import styles from './ProjectsPage.module.css'

export function ProjectsPage() {
  const { t } = useTranslation()
  const { projects, error } = useProjects()
  // The filter lives in the URL (/projects?tag=react): shareable, and the back button works.
  const [searchParams] = useSearchParams()
  const tag = searchParams.get('tag')

  // Few projects: filtering in the browser is enough.
  const shown = tag ? projects?.filter((p) => p.tags.includes(tag)) : projects

  return (
    <>
      <PageHeader command={t('projects.command')} title={t('projects.title')} subtitle={t('projects.subtitle')} />

      {error ? (
        <Alert>{t('projects.loadError', { error })}</Alert>
      ) : !projects || !shown ? (
        <Loader />
      ) : projects.length === 0 ? (
        <EmptyState>{t('projects.empty')}</EmptyState>
      ) : (
        <>
          <TagFilter projects={projects} active={tag} />
          {shown.length === 0 ? (
            <EmptyState>
              {t('projects.noMatch', { tag })} <Link to="/projects">{t('projects.clearFilter')}</Link>
            </EmptyState>
          ) : (
            <>
              <p className={styles.summary}>
                {t('projects.total')} <strong>{shown.length}</strong>
              </p>
              <ProjectGrid projects={shown} activeTag={tag} />
            </>
          )}
        </>
      )}
    </>
  )
}
