import { ChevronLeft, ChevronRight, Search, X } from 'lucide-react'
import { useState, type FormEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { useSearchParams } from 'react-router'

import { Button } from '@/components/ui/button'
import { Input, Label, NativeSelect } from '@/components/ui/input'
import { decodeVin, PAGE_SIZE, useCarBrands, useCategories, useParts, type Category } from '@/features/catalog/api'
import { PartCard } from '@/features/catalog/PartCard'
import { ApiError } from '@/lib/api'

const currentYear = new Date().getFullYear()

export function PartsPage() {
  const { t, i18n } = useTranslation()
  const [params, setParams] = useSearchParams()
  const brands = useCarBrands()
  const categories = useCategories()

  const brandId = params.get('brand') ?? ''
  const modelId = params.get('model') ?? ''
  const year = params.get('year') ?? ''
  const categoryId = params.get('category') ?? ''
  const search = params.get('q') ?? ''
  const page = Math.max(1, Number(params.get('page')) || 1)

  const [searchText, setSearchText] = useState(search)
  const [vin, setVin] = useState('')
  const [vinMessage, setVinMessage] = useState<{ text: string; error?: boolean } | null>(null)

  const brand = brands.data?.find((b) => b.id === brandId)
  const model = brand?.models.find((m) => m.id === modelId)
  const lastYear = model ? Math.min(model.yearTo ?? currentYear, currentYear) : 0
  const years = model ? Array.from({ length: lastYear - model.yearFrom + 1 }, (_, i) => lastYear - i) : []

  const parts = useParts({
    brandId: brandId || undefined,
    carModelId: modelId || undefined,
    year: year ? Number(year) : undefined,
    categoryId: categoryId || undefined,
    search: search || undefined,
    page,
  })

  const update = (changes: Record<string, string>) => {
    const next = new URLSearchParams(params)
    for (const [key, value] of Object.entries(changes)) {
      if (value) next.set(key, value)
      else next.delete(key)
    }
    if (!('page' in changes)) next.delete('page')
    setParams(next)
  }

  // VIN'dan aniqlangan yil yangi model yillari oralig'iga tushsa saqlanadi.
  const onModelChange = (id: string) => {
    const next = brand?.models.find((m) => m.id === id)
    const y = Number(year)
    const keepYear = next && y >= next.yearFrom && y <= (next.yearTo ?? currentYear)
    update({ model: id, year: keepYear ? year : '' })
  }

  const onSearch = (e: FormEvent) => {
    e.preventDefault()
    update({ q: searchText.trim() })
  }

  const onVin = async (e: FormEvent) => {
    e.preventDefault()
    setVinMessage(null)
    try {
      const result = await decodeVin(vin)
      if (!result.brandId) {
        setVinMessage({ text: t('parts.vin.unknownBrand', { wmi: result.wmi }), error: true })
        return
      }
      update({ brand: result.brandId, model: '', year: result.year ? String(result.year) : '' })
      setVinMessage({ text: t('parts.vin.found', { brand: result.brandName, year: result.year ?? '—' }) })
    } catch (err) {
      setVinMessage({ text: err instanceof ApiError && err.status === 400 ? t('parts.vin.invalid') : t('parts.error'), error: true })
    }
  }

  const categoryName = (c: Category) => (i18n.resolvedLanguage === 'ru' ? c.nameRu : c.nameUz)
  const rootCategories = categories.data?.filter((c) => !c.parentId) ?? []
  const hasFilters = !!(brandId || categoryId || search)
  const totalPages = parts.data ? Math.max(1, Math.ceil(parts.data.totalCount / PAGE_SIZE)) : 1

  return (
    <section className="flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">{t('parts.title')}</h1>
        <p className="text-muted-foreground">{t('parts.subtitle')}</p>
      </div>

      <div className="grid gap-4 rounded-xl border bg-card p-4 md:p-6">
        <form onSubmit={onVin} className="flex flex-col gap-2 sm:flex-row sm:items-end">
          <Label className="flex-1">
            {t('parts.vin.label')}
            <Input
              value={vin}
              onChange={(e) => setVin(e.target.value.toUpperCase())}
              placeholder="XWB3L32EDHA012345"
              maxLength={17}
              className="font-mono uppercase"
            />
          </Label>
          <Button type="submit" variant="secondary" disabled={vin.trim().length !== 17}>
            {t('parts.vin.decode')}
          </Button>
        </form>
        {vinMessage && (
          <p className={vinMessage.error ? 'text-sm text-destructive' : 'text-sm text-muted-foreground'}>{vinMessage.text}</p>
        )}

        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <Label>
            {t('parts.filters.brand')}
            <NativeSelect value={brandId} onChange={(e) => update({ brand: e.target.value, model: '', year: '' })}>
              <option value="">{t('parts.filters.any')}</option>
              {brands.data?.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </NativeSelect>
          </Label>
          <Label>
            {t('parts.filters.model')}
            <NativeSelect value={modelId} disabled={!brand} onChange={(e) => onModelChange(e.target.value)}>
              <option value="">{t('parts.filters.any')}</option>
              {brand?.models.map((m) => (
                <option key={m.id} value={m.id}>
                  {m.name}
                </option>
              ))}
            </NativeSelect>
          </Label>
          <Label>
            {t('parts.filters.year')}
            <NativeSelect value={year} disabled={!model} onChange={(e) => update({ year: e.target.value })}>
              <option value="">{t('parts.filters.any')}</option>
              {years.map((y) => (
                <option key={y} value={y}>
                  {y}
                </option>
              ))}
            </NativeSelect>
          </Label>
          <Label>
            {t('parts.filters.category')}
            <NativeSelect value={categoryId} onChange={(e) => update({ category: e.target.value })}>
              <option value="">{t('parts.filters.any')}</option>
              {rootCategories.map((root) => [
                <option key={root.id} value={root.id}>
                  {categoryName(root)}
                </option>,
                ...(categories.data ?? [])
                  .filter((c) => c.parentId === root.id)
                  .map((child) => (
                    <option key={child.id} value={child.id}>
                      {'  — '}
                      {categoryName(child)}
                    </option>
                  )),
              ])}
            </NativeSelect>
          </Label>
        </div>

        <form onSubmit={onSearch} className="flex gap-2">
          <Input
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            placeholder={t('parts.filters.searchPlaceholder')}
            aria-label={t('parts.filters.searchPlaceholder')}
          />
          <Button type="submit" size="icon" aria-label={t('parts.filters.search')}>
            <Search />
          </Button>
          {hasFilters && (
            <Button
              type="button"
              variant="ghost"
              onClick={() => {
                setSearchText('')
                setVinMessage(null)
                setParams(new URLSearchParams())
              }}
            >
              <X />
              {t('parts.filters.reset')}
            </Button>
          )}
        </form>
      </div>

      {parts.isPending && <p className="text-muted-foreground">{t('common.loading')}</p>}
      {parts.isError && <p className="text-destructive">{t('parts.error')}</p>}
      {parts.data && (
        <p className="text-sm text-muted-foreground">{t('parts.found', { count: parts.data.totalCount })}</p>
      )}
      {parts.data?.items.length === 0 && <p className="text-muted-foreground">{t('parts.empty')}</p>}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
        {parts.data?.items.map((part) => <PartCard key={part.id} part={part} />)}
      </div>

      {totalPages > 1 && (
        <nav className="flex items-center justify-center gap-3" aria-label={t('parts.pagination')}>
          <Button variant="outline" size="icon" disabled={page <= 1} onClick={() => update({ page: String(page - 1) })}>
            <ChevronLeft />
          </Button>
          <span className="text-sm">{t('parts.page', { page, total: totalPages })}</span>
          <Button variant="outline" size="icon" disabled={page >= totalPages} onClick={() => update({ page: String(page + 1) })}>
            <ChevronRight />
          </Button>
        </nav>
      )}
    </section>
  )
}
