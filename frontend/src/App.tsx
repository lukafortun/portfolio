import { lazy } from 'react'
import { BrowserRouter, Route, Routes } from 'react-router'
import { AuthProvider } from './auth/AuthProvider'
import { RequireAdmin } from './auth/RequireAdmin'
import { Layout } from './components/layout/Layout'
import { ExperiencePage } from './pages/ExperiencePage'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'
import { NotFoundPage } from './pages/NotFoundPage'
import { ProjectsPage } from './pages/ProjectsPage'
import { SkillsPage } from './pages/SkillsPage'

// Loaded on demand: the project page pulls in the Markdown renderer, and visitors never need the admin.
const ProjectPage = lazy(() => import('./pages/ProjectPage').then((m) => ({ default: m.ProjectPage })))
const AdminLayout = lazy(() => import('./pages/admin/AdminLayout').then((m) => ({ default: m.AdminLayout })))
const AdminProjectsPage = lazy(() =>
  import('./pages/admin/AdminProjectsPage').then((m) => ({ default: m.AdminProjectsPage })),
)
const AdminProfilePage = lazy(() =>
  import('./pages/admin/AdminProfilePage').then((m) => ({ default: m.AdminProfilePage })),
)
const AdminSkillsPage = lazy(() => import('./pages/admin/AdminSkillsPage').then((m) => ({ default: m.AdminSkillsPage })))
const AdminExperiencePage = lazy(() =>
  import('./pages/admin/AdminExperiencePage').then((m) => ({ default: m.AdminExperiencePage })),
)

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<HomePage />} />
            <Route path="projects" element={<ProjectsPage />} />
            <Route path="projects/:slug" element={<ProjectPage />} />
            <Route path="skills" element={<SkillsPage />} />
            <Route path="experience" element={<ExperiencePage />} />
            <Route path="login" element={<LoginPage />} />

            {/* Nested routes: each section renders inside AdminLayout's <Outlet />. */}
            <Route path="admin" element={<RequireAdmin><AdminLayout /></RequireAdmin>}>
              <Route index element={<AdminProjectsPage />} />
              <Route path="profile" element={<AdminProfilePage />} />
              <Route path="skills" element={<AdminSkillsPage />} />
              <Route path="experience" element={<AdminExperiencePage />} />
            </Route>

            <Route path="*" element={<NotFoundPage />} />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}
