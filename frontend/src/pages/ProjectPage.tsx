import { useTranslation } from 'react-i18next'
import { Link, useParams } from 'react-router'
import { Markdown } from '../components/projects/Markdown'
import { Alert } from '../components/ui/Alert'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useProject } from '../hooks/useProject'
import { useLanguage } from '../i18n/useLanguage'
import { formatDate } from '../lib/format'
import { NotFoundPage } from './NotFoundPage'
import styles from './ProjectPage.module.css'

export function ProjectPage() {
  const { t } = useTranslation()
  const language = useLanguage()
  const { slug = '' } = useParams()
  const { project, notFound, error } = useProject(slug)

  if (notFound) return <NotFoundPage />
  if (error) return <Alert>{error}</Alert>
  if (!project) return <Loader />

  return (
    <article lang={project.language}>
      <Link to="/projects" className={styles.back}>{t('project.back')}</Link>

      <PageHeader command={`cat ~/projects/${project.slug}.md`} title={project.title} subtitle={project.summary} />

      <div className={styles.meta}>
        <time dateTime={project.createdAt}>{formatDate(project.createdAt)}</time>
        {project.repoUrl && (
          <a href={project.repoUrl} target="_blank" rel="noreferrer" className={styles.link}>
            {t('project.code')} ↗
          </a>
        )}
        {project.liveUrl && (
          <a href={project.liveUrl} target="_blank" rel="noreferrer" className={styles.link}>
            {t('project.demo')} ↗
          </a>
        )}
      </div>

      {project.coverImage && <img src={project.coverImage} alt="" className={styles.cover} />}

      {project.language !== language && <p className={styles.notice}>{t('project.notTranslated')}</p>}

      {project.content && <Markdown>{project.content}</Markdown>}
    </article>
  )
}
