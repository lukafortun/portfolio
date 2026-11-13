import { ProjectGrid } from '../components/projects/ProjectGrid'
import { Alert } from '../components/ui/Alert'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useProjects } from '../hooks/useProjects'
import styles from './HomePage.module.css'

export function HomePage() {
  const { projects, error } = useProjects()

  return (
    <>
      <PageHeader
        command="ls ~/projets"
        title="Mes projets"
        subtitle="Une sélection de ce que j’ai pu construire."
      />

      {error ? (
        <Alert>Impossible de charger les projets : {error}</Alert>
      ) : !projects ? (
        <Loader />
      ) : projects.length === 0 ? (
        <EmptyState>aucun projet pour l’instant</EmptyState>
      ) : (
        <>
          <p className={styles.summary}>
            total <strong>{projects.length}</strong>
          </p>
          <ProjectGrid projects={projects} />
        </>
      )}
    </>
  )
}
