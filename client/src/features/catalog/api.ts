import { keepPreviousData, useQuery } from '@tanstack/react-query'

import { apiFetch } from '@/lib/api'

export interface CarModel {
  id: string
  name: string
  yearFrom: number
  yearTo: number | null
}

export interface CarBrand {
  id: string
  name: string
  models: CarModel[]
}

export interface Category {
  id: string
  nameUz: string
  nameRu: string
  parentId: string | null
}

export interface PartListItem {
  id: string
  sku: string
  name: string
  manufacturer: string
  isOriginal: boolean
  warrantyMonths: number
  price: number
  stockQuantity: number
  imageUrl: string | null
  categoryId: string
}

export interface PartCompatibility {
  carModelId: string
  brandName: string
  modelName: string
  yearFrom: number
  yearTo: number | null
}

export interface PartDetails extends Omit<PartListItem, 'categoryId'> {
  category: Category
  compatibilities: PartCompatibility[]
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}

export interface VinDecodeResult {
  vin: string
  wmi: string
  brandId: string | null
  brandName: string | null
  year: number | null
}

export interface PartFilters {
  brandId?: string
  carModelId?: string
  year?: number
  categoryId?: string
  search?: string
  page?: number
}

export const PAGE_SIZE = 12

export function useCarBrands() {
  return useQuery({
    queryKey: ['catalog', 'brands'],
    queryFn: () => apiFetch<CarBrand[]>('/api/catalog/brands'),
    staleTime: 10 * 60_000,
  })
}

export function useCategories() {
  return useQuery({
    queryKey: ['catalog', 'categories'],
    queryFn: () => apiFetch<Category[]>('/api/catalog/categories'),
    staleTime: 10 * 60_000,
  })
}

export function useParts(filters: PartFilters) {
  const params = new URLSearchParams({ pageSize: String(PAGE_SIZE) })
  for (const [key, value] of Object.entries(filters)) {
    if (value !== undefined && value !== '') params.set(key, String(value))
  }
  return useQuery({
    queryKey: ['parts', filters],
    queryFn: () => apiFetch<PagedResult<PartListItem>>(`/api/parts?${params}`),
    placeholderData: keepPreviousData,
  })
}

export function usePart(id: string | undefined) {
  return useQuery({
    queryKey: ['parts', id],
    queryFn: () => apiFetch<PartDetails>(`/api/parts/${id}`),
    enabled: !!id,
  })
}

export function decodeVin(vin: string) {
  return apiFetch<VinDecodeResult>(`/api/catalog/vin/${encodeURIComponent(vin.trim())}`)
}
