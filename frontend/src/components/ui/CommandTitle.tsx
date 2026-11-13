import type { ReactNode } from 'react'
import styles from './CommandTitle.module.css'

type Props = {
  /** Shell command shown as the section title, e.g. "ls ~" */
  children: ReactNode
  /** Optional element on the right (count, link…) */
  aside?: ReactNode
}

/** Section title styled as a shell command: "$ ls ~". */
export function CommandTitle({ children, aside }: Props) {
  return (
    <h2 className={styles.title}>
      <span className={styles.prompt} aria-hidden>$</span>
      {children}
      {aside && <span className={styles.aside}>{aside}</span>}
    </h2>
  )
}
