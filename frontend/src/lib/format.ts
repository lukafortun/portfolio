// "sv-SE" formats dates as YYYY-MM-DD, in the local time zone.
const dateFormat = new Intl.DateTimeFormat('sv-SE')

export function formatDate(iso: string) {
  return dateFormat.format(new Date(iso))
}

/** 1 → "01" */
export function padIndex(index: number) {
  return String(index).padStart(2, '0')
}
