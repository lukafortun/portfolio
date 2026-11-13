import type { Profile } from '../../api/content'
import { useLocalize } from '../../i18n/useLocalize'
import { Cursor } from '../ui/Cursor'
import styles from './Hero.module.css'

/** Home page intro: who I am, in a few lines. Edited in the admin (profile tab). */
export function Hero({ profile }: { profile: Profile }) {
  const { l } = useLocalize()
  const location = l(profile.location)

  return (
    <header className={styles.hero}>
      <p className={styles.command}>
        <span className={styles.prompt}>~ $</span> whoami
      </p>

      <h1 className={styles.name}>
        {profile.name}
        <Cursor />
      </h1>

      <p className={styles.role}>
        <span className={styles.arrow} aria-hidden>{'>'}</span> {l(profile.role)}
        {location && <span className={styles.location}> · {location}</span>}
      </p>

      {/* pre-line: keep the line breaks typed in the admin. */}
      <p className={styles.bio}>{l(profile.bio)}</p>

      {(profile.email || profile.links.length > 0) && (
        <ul className={styles.links}>
          {profile.email && (
            <li>
              <a href={`mailto:${profile.email}`} className={`${styles.link} ${styles.email}`}>
                ✉ {profile.email}
              </a>
            </li>
          )}
          {profile.links.map((link) => (
            <li key={link.url}>
              <a href={link.url} target="_blank" rel="noreferrer" className={styles.link}>
                {link.label} ↗
              </a>
            </li>
          ))}
        </ul>
      )}
    </header>
  )
}
