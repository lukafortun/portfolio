import styles from './Cursor.module.css'

/** Static block cursor. */
export function Cursor() {
  return <span className={styles.cursor} aria-hidden />
}
