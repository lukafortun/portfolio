import { request } from './client'

/** Formats accepted by the API (checked there from the file content). */
export const acceptedImageTypes = 'image/png,image/jpeg,image/gif,image/webp'

export const uploadsApi = {
  /** Uploads an image (admin only) and returns its public URL, e.g. "/api/uploads/3f2a….webp". */
  uploadImage: async (file: File) => {
    const form = new FormData()
    form.append('file', file)
    const { url } = await request<{ url: string }>('/uploads', { method: 'POST', body: form })
    return url
  },
}
