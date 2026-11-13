import { Navigate } from 'react-router'
import { authApi } from '../api/auth'
import { useAuth } from '../auth/useAuth'
import { ButtonLink } from '../components/ui/Button'
import { Card } from '../components/ui/Card'
import { Cursor } from '../components/ui/Cursor'
import styles from './LoginPage.module.css'

export function LoginPage() {
  const { email } = useAuth()

  if (email) return <Navigate to="/admin" replace />

  return (
    <Card title="auth — authentik" className={styles.card}>
      <pre className={styles.terminal}>
        <span className={styles.prompt}>~ $</span> sudo admin{'\n'}
        <span className={styles.warn}>[sudo]</span> accès réservé aux administrateurs.{'\n'}
        <span className={styles.warn}>[sudo]</span> authentification requise…{'\n'}
        <span className={styles.prompt}>~ $</span> <Cursor />
      </pre>

      <ButtonLink href={authApi.loginUrl('/admin')} variant="primary" className={styles.button}>
        se connecter avec authentik →
      </ButtonLink>
    </Card>
  )
}
