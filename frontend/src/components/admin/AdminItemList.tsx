import type { ReactNode } from 'react'
import { useTranslation } from 'react-i18next'
import { cx } from '../../lib/cx'
import { padIndex } from '../../lib/format'
import { Button } from '../ui/Button'
import styles from './AdminItemList.module.css'

export type Badge = { label: string; on: boolean }

type Props<T extends { id: number }> = {
  items: T[]
  selectedId: number | undefined
  /** Column headers */
  columns: { title: string; meta: string }
  title: (item: T) => string
  meta?: (item: T) => ReactNode
  badges?: (item: T) => Badge[]
  onEdit: (item: T) => void
  onDelete: (item: T) => void
}

/** Admin table, styled like `ls -l` output: #, title (+ badges), meta, edit/rm buttons. */
export function AdminItemList<T extends { id: number }>({
  items,
  selectedId,
  columns,
  title,
  meta,
  badges,
  onEdit,
  onDelete,
}: Props<T>) {
  const { t } = useTranslation()

  return (
    <div className={styles.table}>
      <div className={cx(styles.row, styles.head)} aria-hidden>
        <span>#</span>
        <span>{columns.title}</span>
        <span className={styles.meta}>{columns.meta}</span>
        <span />
      </div>

      <ul className={styles.list}>
        {items.map((item, i) => (
          <li key={item.id} className={cx(styles.row, item.id === selectedId && styles.selected)}>
            <span className={styles.index}>{padIndex(i + 1)}</span>
            <span className={styles.titleCell}>
              <span className={styles.title}>{title(item)}</span>
              {badges && (
                <span className={styles.badges}>
                  {badges(item).map((badge) => (
                    <span key={badge.label} className={cx(styles.badge, badge.on && styles.badgeOn)}>
                      {badge.label}
                    </span>
                  ))}
                </span>
              )}
            </span>
            <span className={styles.meta}>{meta?.(item)}</span>
            <span className={styles.actions}>
              <Button variant="ghost" size="sm" onClick={() => onEdit(item)}>{t('admin.edit')}</Button>
              <Button
                variant="danger"
                size="sm"
                onClick={() => onDelete(item)}
                aria-label={t('admin.removeLabel', { title: title(item) })}
              >
                {t('admin.remove')}
              </Button>
            </span>
          </li>
        ))}
      </ul>
    </div>
  )
}
