import type { Project } from '../../api/projects'
import { formatDate, padIndex } from '../../lib/format'
import styles from './ProjectCard.module.css'

type Props = {
  project: Project
  /** Position in the list, shown as 01, 02… */
  index: number
}

export function ProjectCard({ project, index }: Props) {
  const { title, description, repoUrl, liveUrl, createdAt } = project

  return (
    <article className={styles.card}>
      <div className={styles.meta}>
        <span className={styles.index}>{padIndex(index)}</span>
        <time dateTime={createdAt}>{formatDate(createdAt)}</time>
      </div>

      <h2 className={styles.title}>{title}</h2>
      <p className={styles.description}>{description}</p>

      {(repoUrl || liveUrl) && (
        <div className={styles.links}>
          {repoUrl && (
            <a href={repoUrl} target="_blank" rel="noreferrer" className={styles.link}>
              code ↗
            </a>
          )}
          {liveUrl && (
            <a href={liveUrl} target="_blank" rel="noreferrer" className={styles.link}>
              démo ↗
            </a>
          )}
        </div>
      )}
    </article>
  )
}
