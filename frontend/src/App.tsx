import { BrowserRouter, Link, Route, Routes } from 'react-router'
import { AuthProvider, RequireAdmin, useAuth } from './auth'
import { AdminPage } from './pages/AdminPage'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'

function Header() {
  const { email, logout } = useAuth()
  return (
    <header className="header">
      <Link to="/" className="brand">Portfolio</Link>
      <nav className="row">
        {email ? (
          <>
            <Link to="/admin">Admin</Link>
            <button onClick={logout}>Déconnexion</button>
          </>
        ) : (
          <Link to="/login">Connexion</Link>
        )}
      </nav>
    </header>
  )
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Header />
        <main className="container">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/admin" element={<RequireAdmin><AdminPage /></RequireAdmin>} />
          </Routes>
        </main>
      </AuthProvider>
    </BrowserRouter>
  )
}
