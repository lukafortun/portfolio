import { useState } from 'react'
import { uploadsApi } from '../api/uploads'

/** Uploads images one by one and tracks progress and errors. */
export function useImageUpload() {
  const [pending, setPending] = useState(0)
  const [error, setError] = useState<string | null>(null)

  /**
   * Uploads the image files among `files` (other files are ignored) and calls `onUploaded` for each one.
   * @returns false if there was no image to upload.
   */
  const upload = (files: Iterable<File>, onUploaded: (url: string, file: File) => void) => {
    const images = [...files].filter((f) => f.type.startsWith('image/'))
    if (images.length === 0) return false

    setError(null)
    for (const file of images) {
      setPending((n) => n + 1)
      uploadsApi
        .uploadImage(file)
        .then((url) => onUploaded(url, file), (e: Error) => setError(e.message))
        .finally(() => setPending((n) => n - 1))
    }
    return true
  }

  return { upload, uploading: pending > 0, error }
}

/** "mon-schema_v2.png" → "mon schema v2" (default alt text). */
export function altFromFileName(name: string) {
  return name.replace(/\.[^.]+$/, '').replace(/[-_]+/g, ' ').trim()
}
