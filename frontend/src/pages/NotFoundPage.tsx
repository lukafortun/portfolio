import { useTranslation } from 'react-i18next'
import { Link, useLocation } from 'react-router'
import { PageHeader } from '../components/ui/PageHeader'
import styles from './NotFoundPage.module.css'

export function NotFoundPage() {
  const { t } = useTranslation()
  const { pathname } = useLocation()

  return (
    <>
      <PageHeader
        command={`cd ${pathname}`}
        title="404"
        subtitle={<span className={styles.error}>{t('notFound.error', { path: pathname })}</span>}
      />
      <Link to="/" className={styles.back}>{t('notFound.back')}</Link>
    </>
  )
}
