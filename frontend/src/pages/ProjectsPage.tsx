import { useTranslation } from 'react-i18next'
import { ProjectGrid } from '../components/projects/ProjectGrid'
import { Alert } from '../components/ui/Alert'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useProjects } from '../hooks/useProjects'
import styles from './ProjectsPage.module.css'

export function ProjectsPage() {
  const { t } = useTranslation()
  const { projects, error } = useProjects()

  return (
    <>
      <PageHeader command={t('projects.command')} title={t('projects.title')} subtitle={t('projects.subtitle')} />

      {error ? (
        <Alert>{t('projects.loadError', { error })}</Alert>
      ) : !projects ? (
        <Loader />
      ) : projects.length === 0 ? (
        <EmptyState>{t('projects.empty')}</EmptyState>
      ) : (
        <>
          <p className={styles.summary}>
            {t('projects.total')} <strong>{projects.length}</strong>
          </p>
          <ProjectGrid projects={projects} />
        </>
      )}
    </>
  )
}
