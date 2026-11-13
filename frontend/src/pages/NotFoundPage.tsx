import { Link, useLocation } from 'react-router'
import { PageHeader } from '../components/ui/PageHeader'
import styles from './NotFoundPage.module.css'

export function NotFoundPage() {
  const { pathname } = useLocation()

  return (
    <>
      <PageHeader
        command={`cd ${pathname}`}
        title="404"
        subtitle={
          <span className={styles.error}>
            bash: cd: {pathname}: No such file or directory
          </span>
        }
      />
      <Link to="/" className={styles.back}>← cd ~</Link>
    </>
  )
}
