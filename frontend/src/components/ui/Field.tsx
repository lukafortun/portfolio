import type { ReactNode } from 'react'
import styles from './Field.module.css'

type Props = {
  label: string
  hint?: string
  /** The input or textarea */
  children: ReactNode
}

export function Field({ label, hint, children }: Props) {
  return (
    <label className={styles.field}>
      <span className={styles.label}>{label}</span>
      {children}
      {hint && <span className={styles.hint}>{hint}</span>}
    </label>
  )
}
