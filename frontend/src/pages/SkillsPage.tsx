import { useTranslation } from 'react-i18next'
import { Alert } from '../components/ui/Alert'
import { Card } from '../components/ui/Card'
import { EmptyState } from '../components/ui/EmptyState'
import { Loader } from '../components/ui/Loader'
import { PageHeader } from '../components/ui/PageHeader'
import { useSkills } from '../hooks/useContent'
import { useLocalize } from '../i18n/useLocalize'
import styles from './SkillsPage.module.css'

export function SkillsPage() {
  const { t } = useTranslation()
  const { l } = useLocalize()
  const { data: groups, error } = useSkills()

  return (
    <>
      <PageHeader command={t('skills.command')} title={t('skills.title')} subtitle={t('skills.subtitle')} />

      {error ? (
        <Alert>{error}</Alert>
      ) : !groups ? (
        <Loader />
      ) : groups.length === 0 ? (
        <EmptyState>{t('skills.empty')}</EmptyState>
      ) : (
        <ul className={styles.groups}>
          {groups.map((group) => (
            <li key={group.id}>
              <Card title={`~/skills/${group.name}`} className={styles.card}>
                {l(group.description) && <p className={styles.description}>{l(group.description)}</p>}
                <ul className={styles.skills}>
                  {group.skills.map((skill) => (
                    <li key={skill} className={styles.skill}>{skill}</li>
                  ))}
                </ul>
              </Card>
            </li>
          ))}
        </ul>
      )}
    </>
  )
}
