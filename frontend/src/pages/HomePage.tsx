import { useEffect, useState } from 'react'
import { api, type Project } from '../api'

export function HomePage() {
  const [projects, setProjects] = useState<Project[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    api.listProjects().then(setProjects, (e: Error) => setError(e.message))
  }, [])

  if (error) return <p className="error">Impossible de charger les projets : {error}</p>
  if (!projects) return <p>Chargement…</p>

  return (
    <section>
      <h1>Projets</h1>
      {projects.length === 0 && <p>Aucun projet pour l’instant.</p>}
      <ul className="cards">
        {projects.map((p) => (
          <li key={p.id} className="card">
            <h2>{p.title}</h2>
            <p>{p.description}</p>
            <div className="links">
              {p.repoUrl && <a href={p.repoUrl} target="_blank" rel="noreferrer">Code</a>}
              {p.liveUrl && <a href={p.liveUrl} target="_blank" rel="noreferrer">Démo</a>}
            </div>
          </li>
        ))}
      </ul>
    </section>
  )
}
