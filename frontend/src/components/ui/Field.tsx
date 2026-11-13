import type { ReactNode } from 'react'
import styles from './Field.module.css'

type Props = {
  label: string
  hint?: string
  /** The input or textarea */
  children: ReactNode
  /**
   * "label" (default) wraps a single input, so clicking the text focuses it.
   * Use "div" when the content has buttons: a <label> would forward clicks on its text to the first one.
   */
  as?: 'label' | 'div'
}

export function Field({ label, hint, children, as: Tag = 'label' }: Props) {
  return (
    <Tag className={styles.field}>
      <span className={styles.label}>{label}</span>
      {children}
      {hint && <span className={styles.hint}>{hint}</span>}
    </Tag>
  )
}
