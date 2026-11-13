import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import type { ProjectListItem } from '../../api/projects'
import { cx } from '../../lib/cx'
import { tagUrl } from '../../lib/tags'
import styles from './TagFilter.module.css'

type Props = {
  /** All the projects (not the filtered ones), to list every tag with its count */
  projects: ProjectListItem[]
  active: string | null
}

/** Every tag used by the projects, most used first; selecting one filters the list (?tag=…). */
export function TagFilter({ projects, active }: Props) {
  const { t } = useTranslation()

  const counts = new Map<string, number>()
  for (const tag of projects.flatMap((p) => p.tags)) counts.set(tag, (counts.get(tag) ?? 0) + 1)
  const tags = [...counts].sort(([a, countA], [b, countB]) => countB - countA || a.localeCompare(b))

  if (tags.length === 0) return null

  return (
    <nav className={styles.filter} aria-label={t('projects.filter')}>
      <span className={styles.prompt} aria-hidden>--tag</span>
      <ul className={styles.options}>
        <li>
          <Link to="/projects" className={cx(styles.option, !active && styles.active)} aria-current={!active || undefined}>
            {t('projects.allTags')}
            <span className={styles.count}>{projects.length}</span>
          </Link>
        </li>
        {tags.map(([tag, count]) => (
          <li key={tag}>
            <Link
              to={tagUrl(tag)}
              className={cx(styles.option, tag === active && styles.active)}
              aria-current={tag === active || undefined}
            >
              <span className={styles.hash} aria-hidden>#</span>
              {tag}
              <span className={styles.count}>{count}</span>
            </Link>
          </li>
        ))}
      </ul>
    </nav>
  )
}
