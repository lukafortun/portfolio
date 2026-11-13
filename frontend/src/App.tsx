import { lazy } from 'react'
import { BrowserRouter, Route, Routes } from 'react-router'
import { AuthProvider } from './auth/AuthProvider'
import { RequireAdmin } from './auth/RequireAdmin'
import { Layout } from './components/layout/Layout'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'
import { NotFoundPage } from './pages/NotFoundPage'

// Loaded on demand: they pull in the Markdown renderer, which the home page doesn't need.
const ProjectPage = lazy(() => import('./pages/ProjectPage').then((m) => ({ default: m.ProjectPage })))
const AdminPage = lazy(() => import('./pages/AdminPage').then((m) => ({ default: m.AdminPage })))

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<HomePage />} />
            <Route path="projects/:slug" element={<ProjectPage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="admin" element={<RequireAdmin><AdminPage /></RequireAdmin>} />
            <Route path="*" element={<NotFoundPage />} />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}
