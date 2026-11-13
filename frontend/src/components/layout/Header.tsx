import { useEffect, useRef, useState, type MouseEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, NavLink, useNavigate } from 'react-router'
import { useAuth } from '../../auth/useAuth'
import { cx } from '../../lib/cx'
import { Cursor } from '../ui/Cursor'
import styles from './Header.module.css'
import { LanguageSwitcher } from './LanguageSwitcher'
import { ThemeSwitcher } from './ThemeSwitcher'

const navLinkClass = ({ isActive }: { isActive: boolean }) => cx(styles.link, isActive && styles.active)

export function Header() {
  const { t } = useTranslation()
  const { email, logout } = useAuth()
  const navigate = useNavigate()
  // Mobile only: the nav is a dropdown panel opened by the menu button.
  const [menuOpen, setMenuOpen] = useState(false)
  const headerRef = useRef<HTMLElement>(null)

  // While the menu is open, Escape or a tap outside the header closes it.
  useEffect(() => {
    if (!menuOpen) return
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') setMenuOpen(false)
    }
    const onPointerDown = (e: PointerEvent) => {
      if (!headerRef.current?.contains(e.target as Node)) setMenuOpen(false)
    }
    document.addEventListener('keydown', onKeyDown)
    document.addEventListener('pointerdown', onPointerDown)
    return () => {
      document.removeEventListener('keydown', onKeyDown)
      document.removeEventListener('pointerdown', onPointerDown)
    }
  }, [menuOpen])

  // Following a link closes the menu (the language and theme pickers keep it open).
  const onNavClick = (e: MouseEvent<HTMLElement>) => {
    if ((e.target as HTMLElement).closest('a')) setMenuOpen(false)
  }

  const onLogout = async () => {
    setMenuOpen(false)
    await logout()
    navigate('/')
  }

  return (
    <header ref={headerRef} className={styles.header}>
      <div className={styles.inner}>
        <Link to="/" className={styles.brand} onClick={() => setMenuOpen(false)}>
          <span className={styles.prompt}>~/</span>portfolio
          <Cursor />
        </Link>

        <button
          type="button"
          className={styles.menuButton}
          aria-expanded={menuOpen}
          aria-controls="main-nav"
          aria-label={menuOpen ? t('nav.closeMenu') : t('nav.openMenu')}
          onClick={() => setMenuOpen((open) => !open)}
        >
          <span className={cx(styles.burger, menuOpen && styles.burgerOpen)} aria-hidden>
            <span />
            <span />
            <span />
          </span>
        </button>

        <nav
          id="main-nav"
          className={cx(styles.nav, menuOpen && styles.navOpen)}
          aria-label={t('nav.label')}
          onClick={onNavClick}
        >
          {/* Not "end": /projects/:slug also highlights "projects". */}
          <NavLink to="/projects" className={navLinkClass}>{t('nav.projects')}</NavLink>
          <NavLink to="/skills" className={navLinkClass}>{t('nav.skills')}</NavLink>
          <NavLink to="/experience" className={navLinkClass}>{t('nav.experience')}</NavLink>
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
          <div className={styles.prefs}>
            <LanguageSwitcher />
            <ThemeSwitcher />
          </div>
        </nav>
      </div>
    </header>
  )
}
