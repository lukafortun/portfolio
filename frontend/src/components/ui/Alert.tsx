import type { ReactNode } from 'react'
import styles from './Alert.module.css'

export function Alert({ children }: { children: ReactNode }) {
  return (
    <p role="alert" className={styles.alert}>
      <span className={styles.tag}>err</span>
      {children}
    </p>
  )
}
