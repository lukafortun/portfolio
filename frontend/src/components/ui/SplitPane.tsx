import {
  useEffect,
  useRef,
  useState,
  type CSSProperties,
  type KeyboardEvent,
  type PointerEvent,
  type ReactNode,
} from 'react'
import { cx } from '../../lib/cx'
import styles from './SplitPane.module.css'

type Props = {
  left: ReactNode
  right: ReactNode
  /** Accessible name of the resize handle */
  label: string
  /** localStorage key to remember the width between visits */
  storageKey: string
  /** Width of the left column, in % of the container */
  defaultPercent?: number
  min?: number
  max?: number
}

function readStoredPercent(key: string) {
  try {
    const value = Number(localStorage.getItem(key))
    return Number.isFinite(value) && value > 0 ? value : null
  } catch {
    return null // storage blocked (private mode…)
  }
}

/**
 * Two columns separated by a draggable handle (mouse, touch or keyboard arrows).
 * Double-click the handle to reset. Below 860px the columns stack and the handle is hidden.
 */
export function SplitPane({ left, right, label, storageKey, defaultPercent = 45, min = 25, max = 70 }: Props) {
  const container = useRef<HTMLDivElement>(null)
  const clamp = (value: number) => Math.min(max, Math.max(min, value))
  const [percent, setPercent] = useState(() => clamp(readStoredPercent(storageKey) ?? defaultPercent))
  const [dragging, setDragging] = useState(false)

  useEffect(() => {
    try {
      localStorage.setItem(storageKey, String(Math.round(percent * 10) / 10))
    } catch {
      // Not critical: the width just won't be remembered.
    }
  }, [percent, storageKey])

  const onPointerDown = (e: PointerEvent<HTMLDivElement>) => {
    e.preventDefault() // no text selection while dragging
    // Keep receiving pointer events even when the pointer leaves the handle.
    e.currentTarget.setPointerCapture(e.pointerId)
    setDragging(true)
  }

  const onPointerMove = (e: PointerEvent<HTMLDivElement>) => {
    if (!dragging || !container.current) return
    const rect = container.current.getBoundingClientRect()
    setPercent(clamp(((e.clientX - rect.left) / rect.width) * 100))
  }

  const onKeyDown = (e: KeyboardEvent<HTMLDivElement>) => {
    const step = e.shiftKey ? 10 : 2
    const next: Record<string, number> = {
      ArrowLeft: percent - step,
      ArrowRight: percent + step,
      Home: min,
      End: max,
    }
    if (!(e.key in next)) return
    e.preventDefault()
    setPercent(clamp(next[e.key]))
  }

  return (
    <div
      ref={container}
      className={cx(styles.split, dragging && styles.dragging)}
      style={{ '--left': `${percent}%` } as CSSProperties}
    >
      <div className={styles.pane}>{left}</div>

      <div
        role="separator"
        aria-orientation="vertical"
        aria-label={label}
        aria-valuenow={Math.round(percent)}
        aria-valuemin={min}
        aria-valuemax={max}
        tabIndex={0}
        className={styles.handle}
        onPointerDown={onPointerDown}
        onPointerMove={onPointerMove}
        onPointerUp={() => setDragging(false)}
        onPointerCancel={() => setDragging(false)}
        onKeyDown={onKeyDown}
        onDoubleClick={() => setPercent(defaultPercent)}
      />

      <div className={styles.pane}>{right}</div>
    </div>
  )
}
