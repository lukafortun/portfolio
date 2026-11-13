import { useRef, useState, type ClipboardEvent, type DragEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { acceptedImageTypes } from '../../api/uploads'
import { altFromFileName, useImageUpload } from '../../hooks/useImageUpload'
import { cx } from '../../lib/cx'
import { Markdown } from '../projects/Markdown'
import { Alert } from '../ui/Alert'
import styles from './MarkdownEditor.module.css'

type Props = {
  /** Form field name: the textarea stays in the form (even in preview), so FormData reads it. */
  name: string
  defaultValue?: string
  label: string
  maxLength?: number
}

/** Markdown textarea with a preview tab, and image upload by paste, drag-and-drop or file picker. */
export function MarkdownEditor({ name, defaultValue = '', label, maxLength }: Props) {
  const { t } = useTranslation()
  const [value, setValue] = useState(defaultValue)
  const [tab, setTab] = useState<'write' | 'preview'>('write')
  const [dragging, setDragging] = useState(false)
  const textarea = useRef<HTMLTextAreaElement>(null)
  const fileInput = useRef<HTMLInputElement>(null)
  const { upload, uploading, error } = useImageUpload()

  // Inserts text at the cursor (replacing the selection), on its own line.
  const insertAtCursor = (text: string) => {
    const el = textarea.current
    const start = el?.selectionStart ?? value.length
    const end = el?.selectionEnd ?? value.length

    setValue((current) => {
      const before = current.slice(0, start)
      const prefix = before === '' || before.endsWith('\n') ? '' : '\n'
      return `${before}${prefix}${text}\n${current.slice(end)}`
    })
  }

  const uploadAndInsert = (files: Iterable<File>) =>
    upload(files, (url, file) => {
      setTab('write')
      insertAtCursor(`![${altFromFileName(file.name)}](${url})`)
    })

  const onPaste = (e: ClipboardEvent<HTMLTextAreaElement>) => {
    // Only take over when the clipboard holds an image (e.g. a screenshot); plain text pastes normally.
    if (uploadAndInsert(e.clipboardData.files)) e.preventDefault()
  }

  const onDragOver = (e: DragEvent) => {
    if (!e.dataTransfer.types.includes('Files')) return
    e.preventDefault()
    setDragging(true)
  }

  const onDrop = (e: DragEvent) => {
    setDragging(false)
    if (e.dataTransfer.files.length === 0) return
    e.preventDefault()
    uploadAndInsert(e.dataTransfer.files)
  }

  return (
    <div className={styles.editor}>
      <div className={styles.toolbar}>
        <div className={styles.tabs} role="tablist">
          {(['write', 'preview'] as const).map((id) => (
            <button
              key={id}
              type="button"
              role="tab"
              aria-selected={tab === id}
              className={cx(styles.tab, tab === id && styles.active)}
              onClick={() => setTab(id)}
            >
              {t(`editor.${id}`)}
            </button>
          ))}
        </div>

        <span className={styles.status} aria-live="polite">
          {uploading && t('editor.uploading')}
        </span>

        <button type="button" className={styles.tool} onClick={() => fileInput.current?.click()}>
          {t('editor.addImage')}
        </button>
        <input
          ref={fileInput}
          type="file"
          accept={acceptedImageTypes}
          multiple
          hidden
          onChange={(e) => {
            uploadAndInsert(e.currentTarget.files ?? [])
            e.currentTarget.value = '' // allow picking the same file again
          }}
        />
      </div>

      <textarea
        ref={textarea}
        name={name}
        aria-label={label}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        onPaste={onPaste}
        onDragOver={onDragOver}
        onDragLeave={() => setDragging(false)}
        onDrop={onDrop}
        maxLength={maxLength}
        rows={16}
        spellCheck
        hidden={tab !== 'write'}
        placeholder={t('editor.placeholder')}
        className={cx(styles.textarea, dragging && styles.dragging)}
      />

      {tab === 'preview' && (
        <div className={styles.preview}>
          {value.trim() ? <Markdown>{value}</Markdown> : <p className={styles.empty}>{t('editor.emptyPreview')}</p>}
        </div>
      )}

      {error && <Alert>{error}</Alert>}
    </div>
  )
}
