import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig({
  plugins: [react()],
  server: {
    // Same origin for the SPA and the API in dev: no CORS, and the auth cookie is sent.
    proxy: {
      '/api': {
        target: 'http://localhost:5071',
        // Keep "Host: localhost:5173": the API builds the Authentik redirect_uri from it.
        // The string shorthand ('/api': 'http://...') would rewrite it to localhost:5071.
        changeOrigin: false,
      },
    },
  },
})
