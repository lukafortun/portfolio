import { useCallback, useEffect, useRef, useState } from 'react'

/**
 * Loads data with `load()` on mount, and again whenever `key` changes (e.g. the language or a slug).
 * `data` is null until the first response; `reload()` fetches again (e.g. after a save).
 */
export function useFetch<T>(load: () => Promise<T>, key = '') {
  const [data, setData] = useState<T | null>(null)
  const [error, setError] = useState<string | null>(null)

  // Always call the latest `load` (callers pass inline functions) without re-fetching on every render:
  // only `key` decides when to fetch.
  const loadRef = useRef(load)
  useEffect(() => {
    loadRef.current = load
  })

  const reload = useCallback(
    () =>
      loadRef.current().then(
        (value) => {
          setData(value)
          setError(null)
        },
        (e: Error) => setError(e.message),
      ),
    [],
  )

  // State is only set once the request resolves; a response arriving after unmount (or after the key changed) is ignored.
  useEffect(() => {
    let ignore = false
    loadRef.current().then(
      (value) => {
        if (ignore) return
        setData(value)
        setError(null)
      },
      (e: Error) => !ignore && setError(e.message),
    )
    return () => {
      ignore = true
    }
  }, [key])

  return { data, error, reload }
}
