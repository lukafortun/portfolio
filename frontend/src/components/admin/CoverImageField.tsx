import { useRef, useState, type DragEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { acceptedImageTypes } from '../../api/uploads'
import { useImageUpload } from '../../hooks/useImageUpload'
import { cx } from '../../lib/cx'
import { Alert } from '../ui/Alert'
import { Button } from '../ui/Button'
import styles from './CoverImageField.module.css'

type Props = {
  /** Form field name: the URL is kept in a hidden input, so FormData reads it. */
  name: string
  defaultValue?: string | null
}

/** Cover image: upload (button or drag-and-drop), preview, remove. */
export function CoverImageField({ name, defaultValue }: Props) {
  const { t } = useTranslation()
  const [url, setUrl] = useState(defaultValue ?? '')
  const [dragging, setDragging] = useState(false)
  const fileInput = useRef<HTMLInputElement>(null)
  const { upload, uploading, error } = useImageUpload()

  // Only one cover: keep the first image.
  const uploadFirst = (files: FileList | null) => upload([...(files ?? [])].slice(0, 1), setUrl)

  const onDrop = (e: DragEvent) => {
    e.preventDefault()
    setDragging(false)
    uploadFirst(e.dataTransfer.files)
  }

  return (
    <div className={styles.field}>
      <input type="hidden" name={name} value={url} />

      <div
        className={cx(styles.frame, dragging && styles.dragging)}
        onDragOver={(e) => {
          e.preventDefault()
          setDragging(true)
        }}
        onDragLeave={() => setDragging(false)}
        onDrop={onDrop}
      >
        {url ? (
          <img src={url} alt="" className={styles.image} />
        ) : (
          <span className={styles.placeholder}>{uploading ? t('editor.uploading') : t('form.coverDrop')}</span>
        )}
      </div>

      <div className={styles.actions}>
        <Button size="sm" onClick={() => fileInput.current?.click()} disabled={uploading}>
          {url ? t('form.coverReplace') : t('form.coverChoose')}
        </Button>
        {url && (
          <Button size="sm" variant="danger" onClick={() => setUrl('')}>
            {t('form.coverRemove')}
          </Button>
        )}
      </div>

      <input
        ref={fileInput}
        type="file"
        accept={acceptedImageTypes}
        hidden
        onChange={(e) => {
          uploadFirst(e.currentTarget.files)
          e.currentTarget.value = ''
        }}
      />

      {error && <Alert>{error}</Alert>}
    </div>
  )
}
