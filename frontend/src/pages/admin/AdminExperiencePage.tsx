import { useTranslation } from 'react-i18next'
import { experiencesApi, type Experience } from '../../api/content'
import { AdminCrud } from '../../components/admin/AdminCrud'
import { AdminItemList } from '../../components/admin/AdminItemList'
import { ExperienceForm } from '../../components/admin/ExperienceForm'
import { useExperiences } from '../../hooks/useContent'
import { useLocalize } from '../../i18n/useLocalize'

export function AdminExperiencePage() {
  const { t } = useTranslation()
  const { l } = useLocalize()
  const { data, error, reload } = useExperiences()

  const label = (x: Experience) => `${l(x.role)} @ ${x.company}`

  return (
    <AdminCrud
      storageKey="admin.experience.formWidth"
      items={data}
      error={error}
      reload={reload}
      remove={experiencesApi.remove}
      itemLabel={label}
      windowTitle={(x) => (x ? t('admin.editWindow', { title: label(x) }) : t('admin.experience.new'))}
      listCommand={t('admin.experience.list')}
      empty={t('admin.experience.empty')}
      renderForm={({ editing, onSaved, onCancel }) => (
        <ExperienceForm
          experience={editing}
          onCancel={onCancel}
          onSubmit={async (input) => {
            if (editing) await experiencesApi.update(editing.id, input)
            else await experiencesApi.create(input)
            await onSaved()
          }}
        />
      )}
      renderList={(props) => (
        <AdminItemList
          {...props}
          columns={t('admin.experience.columns', { returnObjects: true })}
          title={label}
          meta={(x) => `${x.start} → ${x.end ?? '…'}`}
        />
      )}
    />
  )
}
