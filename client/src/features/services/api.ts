import { useQuery } from '@tanstack/react-query'

import { apiFetch } from '@/lib/api'

export interface ServiceType {
  id: string
  nameUz: string
  nameRu: string
  description: string | null
  durationMinutes: number
  priceFrom: number
  priceTo: number
}

export function useServices() {
  return useQuery({
    queryKey: ['services'],
    queryFn: () => apiFetch<ServiceType[]>('/api/services'),
  })
}
