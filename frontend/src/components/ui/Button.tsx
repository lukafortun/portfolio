import type { AnchorHTMLAttributes, ButtonHTMLAttributes } from 'react'
import { cx } from '../../lib/cx'
import styles from './Button.module.css'

type Variant = 'primary' | 'secondary' | 'ghost' | 'danger'
type Size = 'sm' | 'md'
type StyleProps = { variant?: Variant; size?: Size }

function buttonClass({ variant = 'secondary', size = 'md' }: StyleProps, className?: string) {
  return cx(styles.button, styles[variant], styles[size], className)
}

export function Button({
  variant,
  size,
  className,
  type = 'button',
  ...props
}: ButtonHTMLAttributes<HTMLButtonElement> & StyleProps) {
  return <button type={type} className={buttonClass({ variant, size }, className)} {...props} />
}

/** A link that looks like a button, for full-page navigation (e.g. sign-in). */
export function ButtonLink({
  variant,
  size,
  className,
  ...props
}: AnchorHTMLAttributes<HTMLAnchorElement> & StyleProps) {
  return <a className={buttonClass({ variant, size }, className)} {...props} />
}
