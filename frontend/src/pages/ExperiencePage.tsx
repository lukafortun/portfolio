import { useTranslation } from 'react-i18next'
import { tagsApi } from '../api/projects'
import { TagList } from '../components/projects/TagList'
import { Alert } from '../components/ui/Alert'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useExperiences } from '../hooks/useContent'
import { useFetch } from '../hooks/useFetch'
import { useLocalize } from '../i18n/useLocalize'
import { cx } from '../lib/cx'
import styles from './ExperiencePage.module.css'

/** Timeline styled like `git log --graph`: one "commit" per experience. */
export function ExperiencePage() {
  const { t } = useTranslation()
  const { l, month } = useLocalize()
  const { data: experiences, error } = useExperiences()

  // A tag links to the projects that use it, so only when there is at least one (no empty list).
  const { data: tagUsage } = useFetch(tagsApi.list)
  const hasProjects = (tag: string) => tagUsage?.some((u) => u.name === tag && u.projectCount > 0) ?? false

  return (
    <>
      <PageHeader command={t('experience.command')} title={t('experience.title')} subtitle={t('experience.subtitle')} />

      {error ? (
        <Alert>{error}</Alert>
      ) : !experiences ? (
        <Loader />
      ) : experiences.length === 0 ? (
        <EmptyState>{t('experience.empty')}</EmptyState>
      ) : (
        <ol className={styles.timeline}>
          {experiences.map((xp) => (
            <li key={xp.id} className={cx(styles.entry, xp.end === null && styles.current)}>
              <p className={styles.dates}>
                <time dateTime={xp.start}>{month(xp.start)}</time>
                {' → '}
                {xp.end ? (
                  <time dateTime={xp.end}>{month(xp.end)}</time>
                ) : (
                  <span className={styles.present}>{t('experience.present')}</span>
                )}
              </p>

              <h2 className={styles.role}>{l(xp.role)}</h2>
              <p className={styles.company}>
                @ {xp.company}
                {l(xp.location) && <span className={styles.location}> · {l(xp.location)}</span>}
              </p>

              {l(xp.description) && <p className={styles.description}>{l(xp.description)}</p>}

              <TagList tags={xp.tags} isLinked={hasProjects} className={styles.tags} />
            </li>
          ))}
        </ol>
      )}
    </>
  )
}
