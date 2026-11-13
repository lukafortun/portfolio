import { useTranslation } from 'react-i18next'
import { profileApi } from '../../api/content'
import { ProfileForm } from '../../components/admin/ProfileForm'
import { Alert } from '../../components/ui/Alert'
import { Card } from '../../components/ui/Card'
import { Loader } from '../../components/ui/Loader'
import { useProfile } from '../../hooks/useContent'
import styles from './AdminProfilePage.module.css'

/** A single form: there is only one profile. */
export function AdminProfilePage() {
  const { t } = useTranslation()
  const { data: profile, error, reload } = useProfile()

  if (error) return <Alert>{error}</Alert>
  if (!profile) return <Loader />

  return (
    <Card title={t('admin.profile.window')} className={styles.card}>
      <p className={styles.hint}>{t('admin.profile.hint')}</p>
      <ProfileForm
        profile={profile}
        onSubmit={async (input) => {
          await profileApi.save(input)
          await reload()
        }}
      />
    </Card>
  )
}
