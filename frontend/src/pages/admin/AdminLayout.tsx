import { Suspense } from 'react'
import { useTranslation } from 'react-i18next'
import { NavLink, Outlet } from 'react-router'
import { Loader } from '../../components/ui/Loader'
import { PageHeader } from '../../components/ui/PageHeader'
import { cx } from '../../lib/cx'
import styles from './AdminLayout.module.css'

/** Admin shell: header and tabs (styled like tmux windows: "0:projects*"); the section renders in <Outlet />. */
export function AdminLayout() {
  const { t } = useTranslation()

  const tabs = [
    { to: '/admin', end: true, label: t('admin.tabs.projects') },
    { to: '/admin/profile', end: false, label: t('admin.tabs.profile') },
    { to: '/admin/skills', end: false, label: t('admin.tabs.skills') },
    { to: '/admin/experience', end: false, label: t('admin.tabs.experience') },
  ]

  return (
    <>
      <PageHeader command={t('admin.command')} title={t('admin.title')} subtitle={t('admin.subtitle')} />

      <nav className={styles.tabs} aria-label={t('admin.tabsLabel')}>
        {tabs.map((tab, i) => (
          <NavLink
            key={tab.to}
            to={tab.to}
            end={tab.end}
            className={({ isActive }) => cx(styles.tab, isActive && styles.active)}
          >
            <span className={styles.index}>{i}:</span>
            {tab.label}
          </NavLink>
        ))}
      </nav>

      {/* Each section is loaded on demand; keep the header and tabs visible meanwhile. */}
      <Suspense fallback={<Loader />}>
        <Outlet />
      </Suspense>
    </>
  )
}
