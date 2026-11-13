# Portfolio

A personal portfolio site with a small admin area to manage projects.

- **Frontend:** React 19, TypeScript, Vite, React Router
- **Backend:** ASP.NET Core 10 Web API, EF Core 10, PostgreSQL
- **Auth:** sign-in through [Authentik](https://goauthentik.io) (OpenID Connect), session kept in an HttpOnly cookie
- **Infrastructure:** Docker Compose, nginx
- **Tests:** xUnit, NSubstitute

## Architecture

Everything is served from a single URL. nginx serves the frontend and proxies `/api` to the backend:

```
browser ──► web (nginx) ─┬─ /        → React build (static files)
                         └─ /api/*   → api (ASP.NET Core) ──► db (PostgreSQL)
```

```
backend/
  src/Portfolio.Api/          Web API (Controllers → Services → Repositories)
  tests/Portfolio.Api.Tests/  Unit/ and Integration/ tests
  Dockerfile
frontend/
  src/                        React SPA
  nginx.conf                  nginx config used by the web container
  Dockerfile
docker-compose.yml            db + api + web
.env.example                  settings template for Docker Compose
```

## Quick start (Docker)

**Requirements:** [Docker](https://www.docker.com) and an [Authentik](https://goauthentik.io) instance.

### 1. Configure Authentik

1. Create an **OAuth2/OpenID provider**:
   - Client type: **Confidential**
   - Redirect URI: `http://localhost:8080/api/auth/callback`
2. Create an **application** linked to that provider, for example with the slug `portfolio`.
3. Create a group named **`portfolio-admins`** and add the users allowed to access the admin area.

### 2. Configure and run

```bash
cp .env.example .env    # fill in the Authentik values
docker compose up -d --build
```

Open **http://localhost:8080**. Database migrations are applied when the API starts.

### Settings (`.env`)

| Variable | Description | Default |
|---|---|---|
| `APP_PORT` | Port exposed on the host | `8080` |
| `IMAGE_TAG` | Version of the CI images to pull (see [CI](#ci)) | `latest` |
| `POSTGRES_PASSWORD` | PostgreSQL password | `portfolio_dev` |
| `AUTHENTIK_AUTHORITY` | Authentik application URL, e.g. `https://auth.example.com/application/o/portfolio/` | — |
| `AUTHENTIK_CLIENT_ID` | OIDC client ID | — |
| `AUTHENTIK_CLIENT_SECRET` | OIDC client secret | — |

If you change `APP_PORT`, update the redirect URI in Authentik to match.

### Useful commands

```bash
docker compose logs -f api          # follow API logs
docker compose up -d --build        # rebuild after code changes
docker compose down                 # stop (data is kept)
docker compose down -v              # stop and delete all data
```

## Local development

Two ways, both with hot reload: everything in Docker (nothing else to install), or the API and the frontend on the machine (faster rebuilds, debugger in the IDE).

### Option A: everything in Docker

**Requirements:** Docker. For sign-in, the Authentik settings saved with user-secrets (step 2 below) and the redirect URI `http://localhost:5173/api/auth/callback`.

```bash
docker compose -f docker-compose.dev.yml up      # → http://localhost:5173
docker compose -f docker-compose.dev.yml down    # stop (data kept; add -v to delete it)
```

The sources are mounted in the containers: saving a file reloads the page (Vite) or rebuilds the API (`dotnet watch`). The first start takes a few minutes (package downloads). This stack has its own database (port `5433` on the host), separate from the production stack. On Linux/macOS, set `USER_SECRETS_DIR=~/.microsoft/usersecrets` so the API finds the user-secrets.

### Option B: API and frontend on the machine

Run the API and the frontend outside Docker and keep only the database in a container.

**Requirements:** [.NET SDK 10](https://dotnet.microsoft.com/download), [Node.js](https://nodejs.org) 20+, Docker.

1. In the Authentik provider, also add the redirect URI `http://localhost:5173/api/auth/callback`.

2. Store the Authentik settings with [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) (kept outside the repository):

   ```bash
   cd backend
   dotnet user-secrets set "Authentik:Authority"    "https://<your-authentik>/application/o/portfolio/" --project src/Portfolio.Api
   dotnet user-secrets set "Authentik:ClientId"     "<client-id>"     --project src/Portfolio.Api
   dotnet user-secrets set "Authentik:ClientSecret" "<client-secret>" --project src/Portfolio.Api
   ```

3. Start each part:

   ```bash
   # Database
   docker compose up -d db

   # API → http://localhost:5071
   cd backend
   dotnet tool restore
   dotnet run --project src/Portfolio.Api --launch-profile http

   # Frontend → http://localhost:5173 (another terminal)
   cd frontend
   npm install
   npm run dev
   ```

Open **http://localhost:5173**. Vite proxies `/api` to the API, so both share the same origin, as they do behind nginx.

### Tests and tooling

From `backend/`:

```bash
dotnet test
dotnet ef migrations add <Name> --project src/Portfolio.Api --output-dir Data/Migrations
```

From `frontend/`:

```bash
npm run build   # type-check and build
npm run lint
```

### API configuration keys

The API reads standard ASP.NET Core configuration (appsettings, user-secrets, environment variables). Docker Compose maps the `.env` values to these keys.

| Key | Description |
|---|---|
| `ConnectionStrings:Default` | PostgreSQL connection string |
| `Database:MigrateOnStartup` | Apply pending migrations on startup |
| `Authentik:Authority` | Authentik application URL |
| `Authentik:ClientId` | OIDC client ID |
| `Authentik:ClientSecret` | OIDC client secret |

As environment variables, replace `:` with `__` (e.g. `Authentik__ClientSecret`).

## API

| Method | Route | Access |
|---|---|---|
| `GET` | `/api/health` | public |
| `GET` | `/api/projects`, `/api/projects/{id}`, `/api/projects/by-slug/{slug}` | public |
| `POST` `PUT` `DELETE` | `/api/projects`, `/api/projects/{id}` | admin |
| `GET` | `/api/tags` (tags with their number of projects and experience entries) | public |
| `GET` | `/api/profile`, `/api/skills`, `/api/experiences` | public |
| `PUT` | `/api/profile` | admin |
| `POST` `PUT` `DELETE` | `/api/skills`, `/api/skills/{id}`, `/api/experiences`, `/api/experiences/{id}` | admin |
| `POST` | `/api/uploads` (PNG, JPEG, GIF or WebP, 5 MB max) | admin |
| `GET` | `/api/uploads/{file}` | public |
| `GET` | `/api/auth/login` | public (redirects to Authentik) |
| `POST` | `/api/auth/logout` | public |
| `GET` | `/api/auth/me` | admin |

"Admin" means signed in and a member of the `portfolio-admins` group.

All the content (profile, skills, experience and projects) is stored in the database and edited from the admin area (`/admin`). On first start, an empty database is filled with example content to replace.

Each project has a unique slug (its page is `/projects/{slug}`), a short summary for the cards, a Markdown content for its page and tags (`#react`, `#docker`…: stored lowercase without `#`, they filter the project list with `/projects?tag=react`). Experience entries use the same tags (one `Tags` table, linked through `ProjectTags` and `ExperienceTags`); a tag nothing uses anymore is deleted. Texts are translatable: a `fr` translation is required, `en` is optional. `?lang=en` returns them in English, falling back to French when a project has no English translation. In local development, the OpenAPI document is available at `/openapi/v1.json`.

## CI

GitHub Actions ([`.github/workflows/ci.yml`](.github/workflows/ci.yml)) runs on every push and pull request:

1. **Tests:** `dotnet test` for the backend; `npm run lint` and `npm run build` (type-check) for the frontend.
2. **Docker images:** once the tests pass, `api` and `web` are built and pushed to the GitHub Container Registry:

| Event | Tags pushed to `ghcr.io/lukafortun/portfolio-api` and `-web` |
|---|---|
| Push to `main` | `latest`, `sha-<commit>` |
| Tag `v1.2.3` | `1.2.3`, `1.2`, `sha-<commit>` |
| Pull request | none (the images are only built, to check they build) |

Publishing a version:

```bash
git tag v1.0.0
git push origin v1.0.0
```

Deploying the CI images instead of building on the server:

```bash
docker compose pull api web          # IMAGE_TAG in .env picks the version (default: latest)
docker compose up -d
```

New packages on ghcr.io are private: either make them public (package settings on GitHub), or run `docker login ghcr.io` on the server with a token that has the `read:packages` scope.

## Deploying

- The containers serve plain HTTP. Put the stack behind a TLS-terminating reverse proxy (Traefik, Caddy, nginx...). The `X-Forwarded-Proto` header is passed through to the API, so the Authentik redirect URI uses `https`.
- Register the production callback URL `https://<your-domain>/api/auth/callback` in Authentik.
- Set a strong `POSTGRES_PASSWORD`.
- Auth cookie encryption keys are stored in the `dataprotection` volume, so sessions survive container restarts.
- Uploaded images are stored in the `uploads` volume and served at `/api/uploads/…`. Back it up together with the database.
