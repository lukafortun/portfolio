import { Suspense, useEffect } from 'react'
import { useTranslation } from 'react-i18next'
import { Outlet } from 'react-router'
import { useProfile } from '../../hooks/useContent'
import { setSiteTitle } from '../../lib/siteTitle'
import { Loader } from '../ui/Loader'
import { Header } from './Header'
import styles from './Layout.module.css'

const year = new Date().getFullYear()

/** Shell shared by every page; the current route renders in <Outlet />. */
export function Layout() {
  const { t } = useTranslation()
  const { data: profile } = useProfile()

  // The browser tab shows the name configured in the admin profile.
  useEffect(() => {
    if (profile) setSiteTitle(profile.name)
  }, [profile])

  return (
    <div className={styles.shell}>
      <Header />
      <main className={styles.main}>
        {/* Shown while a lazy-loaded page downloads. */}
        <Suspense fallback={<Loader />}>
          <Outlet />
        </Suspense>
      </main>
      {/* Status bar, like at the bottom of an editor or tmux. */}
      <footer className={styles.statusBar}>
        <span className={styles.status}>
          <span className={styles.dot} aria-hidden /> {t('common.online')}
        </span>
        <span>react · asp.net core · postgresql</span>
        <span>© {year}</span>
      </footer>
    </div>
  )
}
