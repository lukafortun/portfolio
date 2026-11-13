import type { Project } from '../../api/projects'
import { cx } from '../../lib/cx'
import { formatDate, padIndex } from '../../lib/format'
import { Button } from '../ui/Button'
import styles from './AdminProjectList.module.css'

type Props = {
  projects: Project[]
  selectedId: number | undefined
  onEdit: (project: Project) => void
  onDelete: (project: Project) => void
}

/** A table-like list, styled like `ls -l` output. */
export function AdminProjectList({ projects, selectedId, onEdit, onDelete }: Props) {
  return (
    <div className={styles.table}>
      <div className={cx(styles.row, styles.head)} aria-hidden>
        <span>#</span>
        <span>titre</span>
        <span className={styles.date}>créé le</span>
        <span />
      </div>

      <ul className={styles.list}>
        {projects.map((project, i) => (
          <li key={project.id} className={cx(styles.row, project.id === selectedId && styles.selected)}>
            <span className={styles.index}>{padIndex(i + 1)}</span>
            <span className={styles.title}>{project.title}</span>
            <span className={styles.date}>{formatDate(project.createdAt)}</span>
            <span className={styles.actions}>
              <Button variant="ghost" size="sm" onClick={() => onEdit(project)}>edit</Button>
              <Button variant="danger" size="sm" onClick={() => onDelete(project)} aria-label={`Supprimer ${project.title}`}>
                rm
              </Button>
            </span>
          </li>
        ))}
      </ul>
    </div>
  )
}
