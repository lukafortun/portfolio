import { useTranslation } from 'react-i18next'
import { Link, NavLink, useNavigate } from 'react-router'
import { useAuth } from '../../auth/useAuth'
import { cx } from '../../lib/cx'
import { Cursor } from '../ui/Cursor'
import styles from './Header.module.css'
import { LanguageSwitcher } from './LanguageSwitcher'

const navLinkClass = ({ isActive }: { isActive: boolean }) => cx(styles.link, isActive && styles.active)

export function Header() {
  const { t } = useTranslation()
  const { email, logout } = useAuth()
  const navigate = useNavigate()

  const onLogout = async () => {
    await logout()
    navigate('/')
  }

  return (
    <header className={styles.header}>
      <div className={styles.inner}>
        <Link to="/" className={styles.brand}>
          <span className={styles.prompt}>~/</span>portfolio
          <Cursor />
        </Link>

        <nav className={styles.nav}>
          <NavLink to="/" end className={navLinkClass}>{t('nav.projects')}</NavLink>
          {email ? (
            <>
              <NavLink to="/admin" className={navLinkClass}>{t('nav.admin')}</NavLink>
              <button type="button" className={cx(styles.link, styles.logout)} onClick={onLogout}>
                {t('nav.logout')}
              </button>
            </>
          ) : (
            <NavLink to="/login" className={navLinkClass}>{t('nav.login')}</NavLink>
          )}
          <LanguageSwitcher />
        </nav>
      </div>
    </header>
  )
}
