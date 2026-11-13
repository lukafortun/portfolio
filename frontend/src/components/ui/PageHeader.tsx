import type { ReactNode } from 'react'
import { Cursor } from './Cursor'
import styles from './PageHeader.module.css'

type Props = {
  title: string
  subtitle?: ReactNode
  /** Shell command shown above the title, e.g. "ls ~/projets" */
  command?: string
}

export function PageHeader({ title, subtitle, command }: Props) {
  return (
    <header className={styles.header}>
      {command && (
        <p className={styles.command}>
          <span className={styles.prompt}>~ $</span> {command}
        </p>
      )}
      <h1 className={styles.title}>
        {title}
        <Cursor />
      </h1>
      {subtitle && <p className={styles.subtitle}>{subtitle}</p>}
    </header>
  )
}
