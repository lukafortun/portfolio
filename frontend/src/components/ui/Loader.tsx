import { useEffect, useState } from 'react'
import styles from './Loader.module.css'

const frames = ['|', '/', '-', '\\']

/** ASCII spinner: | / - \ */
export function Loader({ label = 'chargement' }: { label?: string }) {
  const [frame, setFrame] = useState(0)

  useEffect(() => {
    const id = setInterval(() => setFrame((f) => (f + 1) % frames.length), 120)
    return () => clearInterval(id)
  }, [])

  return (
    <div className={styles.loader} role="status">
      <span className={styles.spinner} aria-hidden>[{frames[frame]}]</span>
      {label}…
    </div>
  )
}
