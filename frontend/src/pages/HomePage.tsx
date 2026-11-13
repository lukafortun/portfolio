import { useTranslation } from 'react-i18next'
import { CategoryCard } from '../components/home/CategoryCard'
import { Hero } from '../components/home/Hero'
import { Alert } from '../components/ui/Alert'
import { CommandTitle } from '../components/ui/CommandTitle'
import { Loader } from '../components/ui/Loader'
import { useExperiences, useProfile, useSkills } from '../hooks/useContent'
import { useProjects } from '../hooks/useProjects'
import styles from './HomePage.module.css'

export function HomePage() {
  const { t } = useTranslation()
  const profile = useProfile()
  const { projects } = useProjects()
  const skills = useSkills()
  const experiences = useExperiences()

  const skillCount = skills.data?.reduce((total, group) => total + group.skills.length, 0)

  return (
    <>
      {profile.error ? <Alert>{profile.error}</Alert> : profile.data ? <Hero profile={profile.data} /> : <Loader />}

      <section className={styles.section}>
        <CommandTitle>{t('home.explore')}</CommandTitle>
        <ul className={styles.categories}>
          <li>
            <CategoryCard
              to="/projects"
              name={t('home.categories.projects.name')}
              description={t('home.categories.projects.description')}
              count={projects ? t('home.categories.projects.count', { count: projects.length }) : undefined}
            />
          </li>
          <li>
            <CategoryCard
              to="/skills"
              name={t('home.categories.skills.name')}
              description={t('home.categories.skills.description')}
              count={skillCount !== undefined ? t('home.categories.skills.count', { count: skillCount }) : undefined}
            />
          </li>
          <li>
            <CategoryCard
              to="/experience"
              name={t('home.categories.experience.name')}
              description={t('home.categories.experience.description')}
              count={
                experiences.data ? t('home.categories.experience.count', { count: experiences.data.length }) : undefined
              }
            />
          </li>
        </ul>
      </section>
    </>
  )
}
