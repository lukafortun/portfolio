import { useTranslation } from 'react-i18next'
import { skillsApi } from '../../api/content'
import { AdminCrud } from '../../components/admin/AdminCrud'
import { AdminItemList } from '../../components/admin/AdminItemList'
import { SkillGroupForm } from '../../components/admin/SkillGroupForm'
import { useSkills } from '../../hooks/useContent'

export function AdminSkillsPage() {
  const { t } = useTranslation()
  const { data, error, reload } = useSkills()

  return (
    <AdminCrud
      storageKey="admin.skills.formWidth"
      items={data}
      error={error}
      reload={reload}
      remove={skillsApi.remove}
      itemLabel={(g) => g.name}
      windowTitle={(g) => (g ? t('admin.editWindow', { title: g.name }) : t('admin.skills.new'))}
      listCommand={t('admin.skills.list')}
      empty={t('admin.skills.empty')}
      renderForm={({ editing, onSaved, onCancel }) => (
        <SkillGroupForm
          group={editing}
          onCancel={onCancel}
          onSubmit={async (input) => {
            if (editing) await skillsApi.update(editing.id, input)
            else await skillsApi.create(input)
            await onSaved()
          }}
        />
      )}
      renderList={(props) => (
        <AdminItemList
          {...props}
          columns={t('admin.skills.columns', { returnObjects: true })}
          title={(g) => `${g.name}/`}
          badges={(g) => [{ label: String(g.skills.length), on: true }]}
          meta={(g) => g.sortOrder}
        />
      )}
    />
  )
}
