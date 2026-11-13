import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import styles from './Loader.module.css'

const frames = ['|', '/', '-', '\\']

/** ASCII spinner: | / - \ */
export function Loader({ label }: { label?: string }) {
  const { t } = useTranslation()
  const [frame, setFrame] = useState(0)

  useEffect(() => {
    const id = setInterval(() => setFrame((f) => (f + 1) % frames.length), 120)
    return () => clearInterval(id)
  }, [])

  return (
    <div className={styles.loader} role="status">
      <span className={styles.spinner} aria-hidden>[{frames[frame]}]</span>
      {label ?? t('common.loading')}…
    </div>
  )
}
