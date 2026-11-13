import { useId, useRef, useState, type ChangeEvent, type KeyboardEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { tagsApi } from '../../api/projects'
import { useFetch } from '../../hooks/useFetch'
import { cx } from '../../lib/cx'
import { normalizeTag } from '../../lib/tags'
import styles from './TagInput.module.css'

type Props = {
  /** Name of the hidden input read by FormData: tags separated by spaces */
  name: string
  /** Accessible name of the text input */
  label: string
  defaultValue?: string[]
  max?: number
}

/** A suggestion: an existing tag, or the typed text as a new one. */
type Option = { name: string; projectCount: number | null }

const maxSuggestions = 8

/**
 * Tags shown as chips, with suggestions from the tags already used by other projects.
 * Keyboard: ↑ ↓ choose a suggestion, Enter / Tab pick it, space or comma validate the typed text,
 * Backspace on an empty input removes the last tag, Escape closes the list.
 */
export function TagInput({ name, label, defaultValue = [], max = 15 }: Props) {
  const { t } = useTranslation()
  const { data: existing } = useFetch(tagsApi.list)

  const [tags, setTags] = useState(defaultValue)
  const [text, setText] = useState('')
  const [open, setOpen] = useState(false)
  const [highlight, setHighlight] = useState(0)
  const inputRef = useRef<HTMLInputElement>(null)
  const listId = useId()

  const query = normalizeTag(text)
  const full = tags.length >= max

  // Existing tags not picked yet that contain the query; those starting with it first
  // (the API already sorts by use, and sort() keeps that order between equals).
  const options: Option[] = (existing ?? [])
    .filter((tag) => !tags.includes(tag.name) && tag.name.includes(query))
    .sort((a, b) => Number(b.name.startsWith(query)) - Number(a.name.startsWith(query)))
    .slice(0, maxSuggestions)
  // The typed text, when it is a new tag.
  if (query && !tags.includes(query) && !existing?.some((tag) => tag.name === query)) {
    options.push({ name: query, projectCount: null })
  }

  const visible = open && !full && options.length > 0
  const active = Math.min(highlight, options.length - 1)
  const optionId = (index: number) => `${listId}-${index}`

  const add = (raw: string) => {
    const tag = normalizeTag(raw)
    if (tag && !tags.includes(tag) && !full) setTags([...tags, tag])
    setText('')
    setHighlight(0)
  }

  const remove = (tag: string) => {
    setTags(tags.filter((x) => x !== tag))
    inputRef.current?.focus()
  }

  const onChange = (e: ChangeEvent<HTMLInputElement>) => {
    // Pasted "#react, #docker vite": validate everything before the last separator.
    const parts = e.target.value.split(/[\s,]+/)
    const last = parts.pop() ?? ''
    if (parts.length > 0) {
      const added = parts.map(normalizeTag).filter((tag) => tag && !tags.includes(tag))
      setTags([...tags, ...new Set(added)].slice(0, max))
    }
    setText(last)
    setHighlight(0)
    setOpen(true)
  }

  const onKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    switch (e.key) {
      case 'ArrowDown':
      case 'ArrowUp': {
        e.preventDefault()
        if (!visible) return setOpen(true)
        const step = e.key === 'ArrowDown' ? 1 : -1
        setHighlight((active + step + options.length) % options.length)
        return
      }
      case 'Enter':
        // Enter would submit the form: only let it through when there is nothing to add.
        if (visible) {
          e.preventDefault()
          add(options[active].name)
        } else if (text) {
          e.preventDefault()
          add(text)
        }
        return
      case 'Tab':
        if (visible && text) {
          e.preventDefault()
          add(options[active].name)
        }
        return
      case ',':
        e.preventDefault()
        add(text)
        return
      case 'Backspace':
        if (!text && tags.length > 0) setTags(tags.slice(0, -1))
        return
      case 'Escape':
        if (visible) {
          e.preventDefault()
          setOpen(false)
        }
        return
    }
  }

  return (
    <div className={styles.wrapper}>
      {/* Clicking anywhere in the box focuses the text input, like a regular field. */}
      <div className={styles.box} onClick={() => inputRef.current?.focus()}>
        {tags.length > 0 && (
          <ul className={styles.chips}>
            {tags.map((tag) => (
              <li key={tag} className={styles.chip}>
                <span className={styles.hash} aria-hidden>#</span>
                {tag}
                <button
                  type="button"
                  className={styles.remove}
                  onClick={() => remove(tag)}
                  aria-label={t('form.removeTag', { tag })}
                  title={t('form.removeTag', { tag })}
                >
                  ×
                </button>
              </li>
            ))}
          </ul>
        )}
        <input
          ref={inputRef}
          className={styles.input}
          value={text}
          onChange={onChange}
          onKeyDown={onKeyDown}
          onFocus={() => setOpen(true)}
          onBlur={() => setOpen(false)}
          disabled={full}
          placeholder={full ? t('form.tagsFull', { max }) : tags.length === 0 ? '#react #dotnet #docker' : ''}
          spellCheck={false}
          autoComplete="off"
          role="combobox"
          aria-label={label}
          aria-autocomplete="list"
          aria-expanded={visible}
          aria-controls={listId}
          aria-activedescendant={visible ? optionId(active) : undefined}
        />
      </div>

      {visible && (
        <ul id={listId} role="listbox" aria-label={label} className={styles.suggestions}>
          {options.map((option, i) => (
            <li
              key={option.name}
              id={optionId(i)}
              role="option"
              aria-selected={i === active}
              className={cx(styles.option, i === active && styles.active)}
              // mousedown would blur the input (and close the list) before the click.
              onMouseDown={(e) => e.preventDefault()}
              onClick={() => add(option.name)}
              onMouseEnter={() => setHighlight(i)}
            >
              <span>
                <span className={styles.hash} aria-hidden>#</span>
                {option.name}
              </span>
              <span className={styles.count}>
                {option.projectCount === null
                  ? t('form.newTag')
                  : t('form.tagUses', { count: option.projectCount })}
              </span>
            </li>
          ))}
        </ul>
      )}

      {/* What FormData reads. The text being typed is included, so it is not lost if the form is submitted right away. */}
      <input type="hidden" name={name} value={[...tags, text].join(' ')} />
    </div>
  )
}
