import { useId } from 'react'
import type { Language } from '../../i18n'

// SVG flags: emoji flags don't render on Windows (they show as "FR" / "GB" letters).

/** Flag for a UI language. */
export function Flag({ language }: { language: Language }) {
  return language === 'fr' ? <FlagFr /> : <FlagGb />
}

export function FlagFr() {
  return (
    <svg viewBox="0 0 3 2" width="1.5em" height="1em" aria-hidden>
      <rect width="1" height="2" fill="#002654" />
      <rect x="1" width="1" height="2" fill="#fff" />
      <rect x="2" width="1" height="2" fill="#ce1126" />
    </svg>
  )
}

export function FlagGb() {
  // Unique ids: the flag may be rendered several times on the same page.
  const id = useId()
  const clipFlag = `${id}-flag`
  const clipDiagonals = `${id}-diagonals`

  return (
    <svg viewBox="0 0 60 30" width="1.5em" height="0.75em" aria-hidden>
      <clipPath id={clipFlag}>
        <path d="M0,0 v30 h60 v-30 z" />
      </clipPath>
      <clipPath id={clipDiagonals}>
        <path d="M30,15 h30 v15 z v15 h-30 z h-30 v-15 z v-15 h30 z" />
      </clipPath>
      <g clipPath={`url(#${clipFlag})`}>
        <path d="M0,0 v30 h60 v-30 z" fill="#012169" />
        <path d="M0,0 L60,30 M60,0 L0,30" stroke="#fff" strokeWidth="6" />
        <path d="M0,0 L60,30 M60,0 L0,30" clipPath={`url(#${clipDiagonals})`} stroke="#c8102e" strokeWidth="4" />
        <path d="M30,0 v30 M0,15 h60" stroke="#fff" strokeWidth="10" />
        <path d="M30,0 v30 M0,15 h60" stroke="#c8102e" strokeWidth="6" />
      </g>
    </svg>
  )
}
