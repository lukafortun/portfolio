import { useTranslation } from 'react-i18next'
import { Navigate } from 'react-router'
import { authApi } from '../api/auth'
import { useAuth } from '../auth/useAuth'
import { ButtonLink } from '../components/ui/Button'
import { Card } from '../components/ui/Card'
import { Cursor } from '../components/ui/Cursor'
import styles from './LoginPage.module.css'

export function LoginPage() {
  const { t } = useTranslation()
  const { email } = useAuth()

  if (email) return <Navigate to="/admin" replace />

  return (
    <Card title={t('login.window')} className={styles.card}>
      <pre className={styles.terminal}>
        <span className={styles.prompt}>~ $</span> sudo admin{'\n'}
        <span className={styles.warn}>[sudo]</span> {t('login.restricted')}{'\n'}
        <span className={styles.warn}>[sudo]</span> {t('login.required')}{'\n'}
        <span className={styles.prompt}>~ $</span> <Cursor />
      </pre>

      <ButtonLink href={authApi.loginUrl('/admin')} variant="primary" className={styles.button}>
        {t('login.button')}
      </ButtonLink>
    </Card>
  )
}
