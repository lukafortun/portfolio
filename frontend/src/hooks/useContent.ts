import { experiencesApi, profileApi, skillsApi } from '../api/content'
import { useFetch } from './useFetch'

export const useProfile = () => useFetch(profileApi.get)
export const useSkills = () => useFetch(skillsApi.list)
export const useExperiences = () => useFetch(experiencesApi.list)
