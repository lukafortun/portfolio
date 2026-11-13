import { useTranslation } from 'react-i18next'
import { ProjectGrid } from '../components/projects/ProjectGrid'
import { Alert } from '../components/ui/Alert'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useProjects } from '../hooks/useProjects'
import styles from './HomePage.module.css'

export function HomePage() {
  const { t } = useTranslation()
  const { projects, error } = useProjects()

  return (
    <>
      <PageHeader command={t('home.command')} title={t('home.title')} subtitle={t('home.subtitle')} />

      {error ? (
        <Alert>{t('home.loadError', { error })}</Alert>
      ) : !projects ? (
        <Loader />
      ) : projects.length === 0 ? (
        <EmptyState>{t('home.empty')}</EmptyState>
      ) : (
        <>
          <p className={styles.summary}>
            {t('home.total')} <strong>{projects.length}</strong>
          </p>
          <ProjectGrid projects={projects} />
        </>
      )}
    </>
  )
}
