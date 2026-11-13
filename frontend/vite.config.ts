import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// Both are set by docker-compose.dev.yml; without them (npm run dev on the host), the defaults apply.
// API_URL: where the API runs ("http://api:5071" inside Docker).
const apiUrl = process.env.API_URL ?? 'http://localhost:5071'
// WATCH_POLLING: file change events don't reach a container from a Windows/macOS folder: poll instead.
const polling = process.env.WATCH_POLLING === 'true'

export default defineConfig({
  plugins: [react()],
  server: {
    watch: polling ? { usePolling: true, interval: 300 } : undefined,
    // Same origin for the SPA and the API in dev: no CORS, and the auth cookie is sent.
    proxy: {
      '/api': {
        target: apiUrl,
        // Keep "Host: localhost:5173": the API builds the Authentik redirect_uri from it.
        // The string shorthand ('/api': 'http://...') would rewrite it to localhost:5071.
        changeOrigin: false,
      },
    },
  },
})
