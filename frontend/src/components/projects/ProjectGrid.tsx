import type { ProjectListItem } from '../../api/projects'
import { ProjectCard } from './ProjectCard'
import styles from './ProjectGrid.module.css'

type Props = {
  projects: ProjectListItem[]
  /** Tag currently used to filter the list, highlighted on the cards */
  activeTag?: string | null
}

export function ProjectGrid({ projects, activeTag }: Props) {
  return (
    <ul className={styles.grid}>
      {projects.map((project, i) => (
        <li key={project.id}>
          <ProjectCard project={project} index={i + 1} activeTag={activeTag} />
        </li>
      ))}
    </ul>
  )
}
