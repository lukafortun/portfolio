import ReactMarkdown, { type Components } from 'react-markdown'
import remarkGfm from 'remark-gfm'
import styles from './Markdown.module.css'

// Raw HTML in the Markdown is not rendered (react-markdown's default), so content can't inject scripts.
const components: Components = {
  // External links open in a new tab.
  a: ({ node: _node, href, ...props }) =>
    href?.startsWith('http') ? (
      <a href={href} target="_blank" rel="noreferrer" {...props} />
    ) : (
      <a href={href} {...props} />
    ),
  img: ({ node: _node, alt, ...props }) => <img alt={alt ?? ''} loading="lazy" {...props} />,
}

/** Renders Markdown (with GitHub extensions: tables, task lists, strikethrough, autolinks). */
export function Markdown({ children }: { children: string }) {
  return (
    <div className={styles.prose}>
      <ReactMarkdown remarkPlugins={[remarkGfm]} components={components}>
        {children}
      </ReactMarkdown>
    </div>
  )
}
