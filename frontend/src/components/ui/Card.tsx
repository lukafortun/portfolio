import type { HTMLAttributes } from 'react'
import { cx } from '../../lib/cx'
import styles from './Card.module.css'

type Props = HTMLAttributes<HTMLDivElement> & {
  /** Shown in a terminal-window title bar */
  title?: string
}

export function Card({ title, className, children, ...props }: Props) {
  return (
    <div className={cx(styles.card, className)} {...props}>
      {title && (
        <div className={styles.bar}>
          <span className={styles.dots} aria-hidden>
            <i /><i /><i />
          </span>
          <span className={styles.title}>{title}</span>
        </div>
      )}
      <div className={styles.body}>{children}</div>
    </div>
  )
}
