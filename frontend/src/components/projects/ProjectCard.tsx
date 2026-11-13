import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import type { ProjectListItem } from '../../api/projects'
import { formatDate, padIndex } from '../../lib/format'
import styles from './ProjectCard.module.css'

type Props = {
  project: ProjectListItem
  /** Position in the list, shown as 01, 02… */
  index: number
}

export function ProjectCard({ project, index }: Props) {
  const { t } = useTranslation()
  const { slug, title, summary, repoUrl, liveUrl, coverImage, createdAt } = project

  return (
    // lang: the text may be in the fallback language rather than the selected one.
    <article className={styles.card} lang={project.language}>
      {/* Decorative (alt=""): the title right below already says what it is. */}
      {coverImage && <img src={coverImage} alt="" loading="lazy" className={styles.cover} />}

      <div className={styles.meta}>
        <span className={styles.index}>{padIndex(index)}</span>
        <time dateTime={createdAt}>{formatDate(createdAt)}</time>
      </div>

      <h2 className={styles.title}>
        {/* Its ::after covers the whole card, so the card is clickable (see the CSS). */}
        <Link to={`/projects/${slug}`} className={styles.titleLink}>{title}</Link>
      </h2>
      <p className={styles.summary}>{summary}</p>

      <div className={styles.links}>
        {repoUrl && (
          <a href={repoUrl} target="_blank" rel="noreferrer" className={styles.link}>
            {t('project.code')} ↗
          </a>
        )}
        {liveUrl && (
          <a href={liveUrl} target="_blank" rel="noreferrer" className={styles.link}>
            {t('project.demo')} ↗
          </a>
        )}
        <span className={styles.read} aria-hidden>{t('project.read')} →</span>
      </div>
    </article>
  )
}
