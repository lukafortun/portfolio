import { Fragment, useState, type ReactNode } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert } from '../ui/Alert'
import { Card } from '../ui/Card'
import { CommandTitle } from '../ui/CommandTitle'
import { EmptyState } from '../ui/EmptyState'
import { Loader } from '../ui/Loader'
import { SplitPane } from '../ui/SplitPane'
import styles from './AdminCrud.module.css'

type FormProps<TFull> = {
  /** The item being edited, or null to create a new one */
  editing: TFull | null
  /** Call after a successful save: closes the edit, empties the form and reloads the list. */
  onSaved: () => Promise<void>
  onCancel: () => void
}

type ListProps<TItem> = {
  items: TItem[]
  selectedId: number | undefined
  onEdit: (item: TItem) => void
  onDelete: (item: TItem) => void
}

type Props<TItem extends { id: number }, TFull extends { id: number }> = {
  /** localStorage key for the column width */
  storageKey: string
  items: TItem[] | null
  error: string | null
  reload: () => Promise<unknown>
  /**
   * Loads the complete item before editing it (projects: the list has no Markdown content).
   * Without it, the list item itself is edited.
   */
  load?: (item: TItem) => Promise<TFull>
  remove: (id: number) => Promise<unknown>
  /** Name shown in the delete confirmation */
  itemLabel: (item: TItem) => string
  /** Title bar of the form window */
  windowTitle: (editing: TFull | null) => string
  listCommand: string
  empty: string
  renderForm: (props: FormProps<TFull>) => ReactNode
  renderList: (props: ListProps<TItem>) => ReactNode
}

/**
 * Admin page layout shared by projects, skills and experience:
 * a create/edit form on the left, the list on the right, with a resizable split.
 */
export function AdminCrud<TItem extends { id: number }, TFull extends { id: number } = TItem>({
  storageKey,
  items,
  error,
  reload,
  load,
  remove,
  itemLabel,
  windowTitle,
  listCommand,
  empty,
  renderForm,
  renderList,
}: Props<TItem, TFull>) {
  const { t } = useTranslation()
  const [editing, setEditing] = useState<TFull | null>(null)
  // Bumped after each save: a new form key remounts the form, which empties it.
  const [formVersion, setFormVersion] = useState(0)
  const [actionError, setActionError] = useState<string | null>(null)

  const edit = async (item: TItem) => {
    try {
      setActionError(null)
      // Without `load`, TItem and TFull are the same type (the default type argument).
      setEditing(load ? await load(item) : (item as unknown as TFull))
    } catch (e) {
      setActionError((e as Error).message)
    }
  }

  const onSaved = async () => {
    setEditing(null)
    setFormVersion((v) => v + 1)
    await reload()
  }

  const onDelete = async (item: TItem) => {
    if (!confirm(t('admin.confirmDelete', { title: itemLabel(item) }))) return
    try {
      setActionError(null)
      await remove(item.id)
      if (editing?.id === item.id) setEditing(null)
      await reload()
    } catch (e) {
      setActionError((e as Error).message)
    }
  }

  return (
    <SplitPane
      label={t('admin.resize')}
      storageKey={storageKey}
      left={
        <Card title={windowTitle(editing)} className={styles.formCard}>
          <Fragment key={`${editing?.id ?? 'new'}-${formVersion}`}>
            {renderForm({ editing, onSaved, onCancel: () => setEditing(null) })}
          </Fragment>
        </Card>
      }
      right={
        <section className={styles.listSection}>
          <CommandTitle aside={<span className={styles.count}>{items?.length ?? 0}</span>}>{listCommand}</CommandTitle>

          {actionError && <Alert>{actionError}</Alert>}

          {error ? (
            <Alert>{error}</Alert>
          ) : !items ? (
            <Loader />
          ) : items.length === 0 ? (
            <EmptyState>{empty}</EmptyState>
          ) : (
            renderList({ items, selectedId: editing?.id, onEdit: edit, onDelete })
          )}
        </section>
      }
    />
  )
}
