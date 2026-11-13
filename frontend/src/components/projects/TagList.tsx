import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { cx } from '../../lib/cx'
import { tagUrl } from '../../lib/tags'
import styles from './TagList.module.css'

type Props = {
  tags: string[]
  /** Highlighted tag (the current filter) */
  active?: string | null
  className?: string
}

/** "#react #docker": each tag links to the projects that have it. */
export function TagList({ tags, active, className }: Props) {
  const { t } = useTranslation()
  if (tags.length === 0) return null

  return (
    <ul className={cx(styles.tags, className)} aria-label={t('projects.tags')}>
      {tags.map((tag) => (
        <li key={tag}>
          <Link
            to={tagUrl(tag)}
            className={cx(styles.tag, tag === active && styles.active)}
            aria-current={tag === active || undefined}
          >
            <span className={styles.hash} aria-hidden>#</span>
            {tag}
          </Link>
        </li>
      ))}
    </ul>
  )
}
