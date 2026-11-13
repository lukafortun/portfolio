import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import styles from './CategoryCard.module.css'

type Props = {
  to: string
  /** Shown as a folder: "projects/" */
  name: string
  description: string
  /** e.g. "4 projects"; undefined while loading */
  count: string | undefined
}

/** A home page entry leading to a section, styled like a folder in `ls -l` output. */
export function CategoryCard({ to, name, description, count }: Props) {
  const { t } = useTranslation()

  return (
    <Link to={to} className={styles.card}>
      <span className={styles.permissions} aria-hidden>drwxr-xr-x</span>
      <span className={styles.name}>{name}/</span>
      <span className={styles.description}>{description}</span>
      <span className={styles.footer}>
        <span>{count ?? '…'}</span>
        <span className={styles.open}>{t('home.open')} →</span>
      </span>
    </Link>
  )
}
