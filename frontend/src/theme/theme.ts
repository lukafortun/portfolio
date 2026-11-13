/** Color palettes, defined in styles/global.css as [data-theme='…'] blocks. */
export const themes = ['phosphor', 'amber', 'ice', 'synth', 'paper'] as const
export type Theme = (typeof themes)[number]

/** 'auto' follows the system setting: paper when it is light, phosphor when it is dark. */
export type ThemeChoice = Theme | 'auto'

// Keep in sync with the inline script in index.html, which applies the theme before the first paint.
const storageKey = 'theme'
const lightQuery = '(prefers-color-scheme: light)'

function isTheme(value: string | null): value is Theme {
  return themes.includes(value as Theme)
}

export function readChoice(): ThemeChoice {
  try {
    const saved = localStorage.getItem(storageKey)
    if (isTheme(saved)) return saved
  } catch {
    // Storage blocked (private mode…): fall back to auto.
  }
  return 'auto'
}

export function saveChoice(choice: ThemeChoice) {
  try {
    if (choice === 'auto') localStorage.removeItem(storageKey)
    else localStorage.setItem(storageKey, choice)
  } catch {
    // Not saved: the choice still applies until the page is reloaded.
  }
}

export function resolveTheme(choice: ThemeChoice): Theme {
  if (choice !== 'auto') return choice
  return matchMedia(lightQuery).matches ? 'paper' : 'phosphor'
}

export function applyTheme(theme: Theme) {
  const root = document.documentElement
  root.dataset.theme = theme
  // The browser UI (mobile address bar) takes the page background color.
  const background = getComputedStyle(root).getPropertyValue('--bg').trim()
  document.querySelector('meta[name="theme-color"]')?.setAttribute('content', background)
}

/** Calls `onChange` when the system switches between light and dark. Returns the unsubscribe function. */
export function watchSystemTheme(onChange: () => void) {
  const query = matchMedia(lightQuery)
  query.addEventListener('change', onChange)
  return () => query.removeEventListener('change', onChange)
}
