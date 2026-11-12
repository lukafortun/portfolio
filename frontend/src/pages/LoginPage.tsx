import { Navigate } from 'react-router'
import { api } from '../api'
import { useAuth } from '../auth'

export function LoginPage() {
  const { email } = useAuth()

  if (email) return <Navigate to="/admin" replace />

  return (
    <section className="narrow">
      <h1>Connexion admin</h1>
      <p>La connexion se fait via Authentik.</p>
      <a href={api.loginUrl('/admin')} className="button">Se connecter avec Authentik</a>
    </section>
  )
}
