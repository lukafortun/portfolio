import { useTranslation } from 'react-i18next'
import type { ProjectListItem } from '../../api/projects'
import { languages } from '../../i18n'
import { cx } from '../../lib/cx'
import { formatDate, padIndex } from '../../lib/format'
import { Button } from '../ui/Button'
import styles from './AdminProjectList.module.css'

type Props = {
  projects: ProjectListItem[]
  selectedId: number | undefined
  onEdit: (project: ProjectListItem) => void
  onDelete: (project: ProjectListItem) => void
}

/** A table-like list, styled like `ls -l` output. */
export function AdminProjectList({ projects, selectedId, onEdit, onDelete }: Props) {
  const { t } = useTranslation()

  return (
    <div className={styles.table}>
      <div className={cx(styles.row, styles.head)} aria-hidden>
        <span>#</span>
        <span>{t('admin.columns.title')}</span>
        <span className={styles.date}>{t('admin.columns.createdAt')}</span>
        <span />
      </div>

      <ul className={styles.list}>
        {projects.map((project, i) => (
          <li key={project.id} className={cx(styles.row, project.id === selectedId && styles.selected)}>
            <span className={styles.index}>{padIndex(i + 1)}</span>
            <span className={styles.titleCell}>
              <span className={styles.title}>{project.title}</span>
              {/* Which languages are filled in, e.g. [fr][en] */}
              <span className={styles.langs}>
                {languages.map((lng) => (
                  <span
                    key={lng}
                    className={cx(styles.lang, project.languages.includes(lng) && styles.langDone)}
                  >
                    {lng}
                  </span>
                ))}
              </span>
            </span>
            <span className={styles.date}>{formatDate(project.createdAt)}</span>
            <span className={styles.actions}>
              <Button variant="ghost" size="sm" onClick={() => onEdit(project)}>{t('admin.edit')}</Button>
              <Button
                variant="danger"
                size="sm"
                onClick={() => onDelete(project)}
                aria-label={t('admin.removeLabel', { title: project.title })}
              >
                {t('admin.remove')}
              </Button>
            </span>
          </li>
        ))}
      </ul>
    </div>
  )
}
