import type { Project } from '../../api/projects'
import { ProjectCard } from './ProjectCard'
import styles from './ProjectGrid.module.css'

export function ProjectGrid({ projects }: { projects: Project[] }) {
  return (
    <ul className={styles.grid}>
      {projects.map((project, i) => (
        <li key={project.id}>
          <ProjectCard project={project} index={i + 1} />
        </li>
      ))}
    </ul>
  )
}
