import { ArrowLeft, Package, ShieldCheck } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { Link, useParams } from 'react-router'

import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { usePart } from '@/features/catalog/api'
import { StockBadge } from '@/features/catalog/PartCard'
import { ApiError } from '@/lib/api'
import { formatSum } from '@/lib/utils'

export function PartPage() {
  const { id } = useParams()
  const { t, i18n } = useTranslation()
  const { data: part, isPending, error } = usePart(id)

  const back = (
    <Button asChild variant="ghost" size="sm" className="self-start">
      <Link to="/parts">
        <ArrowLeft />
        {t('parts.backToCatalog')}
      </Link>
    </Button>
  )

  if (isPending) return <p className="text-muted-foreground">{t('common.loading')}</p>
  if (error || !part)
    return (
      <section className="flex flex-col gap-4">
        {back}
        <p className={error instanceof ApiError && error.status === 404 ? 'text-muted-foreground' : 'text-destructive'}>
          {error instanceof ApiError && error.status === 404 ? t('parts.notFound') : t('parts.error')}
        </p>
      </section>
    )

  const category = i18n.resolvedLanguage === 'ru' ? part.category.nameRu : part.category.nameUz

  return (
    <section className="flex flex-col gap-6">
      {back}
      <div className="grid gap-8 md:grid-cols-2">
        <div className="flex aspect-square items-center justify-center overflow-hidden rounded-xl border bg-muted">
          {part.imageUrl ? (
            <img src={part.imageUrl} alt={part.name} className="size-full object-cover" />
          ) : (
            <Package className="size-16 text-muted-foreground" />
          )}
        </div>
        <div className="flex flex-col gap-4">
          <p className="text-sm text-muted-foreground">{category}</p>
          <h1 className="text-3xl font-bold tracking-tight">{part.name}</h1>
          <div className="flex flex-wrap gap-2">
            {part.isOriginal ? <Badge>{t('parts.original')}</Badge> : <Badge variant="secondary">{t('parts.analog')}</Badge>}
            <StockBadge quantity={part.stockQuantity} />
          </div>
          <p className="text-3xl font-semibold">{t('parts.price', { price: formatSum(part.price) })}</p>
          <dl className="grid grid-cols-[auto_1fr] gap-x-6 gap-y-2 text-sm">
            <dt className="text-muted-foreground">{t('parts.sku')}</dt>
            <dd className="font-mono">{part.sku}</dd>
            <dt className="text-muted-foreground">{t('parts.manufacturer')}</dt>
            <dd>{part.manufacturer}</dd>
            <dt className="text-muted-foreground">{t('parts.stock')}</dt>
            <dd>{t('parts.stockCount', { count: part.stockQuantity })}</dd>
          </dl>
          {part.warrantyMonths > 0 && (
            <p className="flex items-center gap-2 text-sm">
              <ShieldCheck className="size-4 text-primary" />
              {t('parts.warranty', { count: part.warrantyMonths })}
            </p>
          )}
          <Button disabled className="self-start" title={t('common.comingSoon')}>
            {t('parts.addToCart')} · {t('common.comingSoon')}
          </Button>
        </div>
      </div>

      <div className="flex flex-col gap-3">
        <h2 className="text-xl font-semibold">{t('parts.compatibility.title')}</h2>
        {part.compatibilities.length === 0 ? (
          <p className="text-muted-foreground">{t('parts.compatibility.empty')}</p>
        ) : (
          <div className="overflow-x-auto rounded-xl border">
            <table className="w-full text-sm">
              <thead className="bg-muted text-left">
                <tr>
                  <th className="px-4 py-2 font-medium">{t('parts.filters.brand')}</th>
                  <th className="px-4 py-2 font-medium">{t('parts.filters.model')}</th>
                  <th className="px-4 py-2 font-medium">{t('parts.compatibility.years')}</th>
                </tr>
              </thead>
              <tbody>
                {part.compatibilities.map((c) => (
                  <tr key={c.carModelId} className="border-t">
                    <td className="px-4 py-2">{c.brandName}</td>
                    <td className="px-4 py-2">{c.modelName}</td>
                    <td className="px-4 py-2">
                      {c.yearFrom} – {c.yearTo ?? t('parts.compatibility.present')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </section>
  )
}
