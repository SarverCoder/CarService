import { Package } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'

import { Badge } from '@/components/ui/badge'
import { Card, CardDescription, CardTitle } from '@/components/ui/card'
import { formatSum } from '@/lib/utils'

import type { PartListItem } from './api'

export function StockBadge({ quantity }: { quantity: number }) {
  const { t } = useTranslation()
  return quantity > 0 ? (
    <Badge variant="success">{t('parts.inStock')}</Badge>
  ) : (
    <Badge variant="destructive">{t('parts.outOfStock')}</Badge>
  )
}

export function PartCard({ part }: { part: PartListItem }) {
  const { t } = useTranslation()
  return (
    <Link to={`/parts/${part.id}`} className="group rounded-xl outline-none focus-visible:ring-[3px] focus-visible:ring-ring/50">
      <Card className="h-full gap-3 transition-shadow group-hover:shadow-md">
        <div className="flex aspect-[4/3] items-center justify-center overflow-hidden rounded-lg bg-muted">
          {part.imageUrl ? (
            <img src={part.imageUrl} alt={part.name} className="size-full object-cover" loading="lazy" />
          ) : (
            <Package className="size-10 text-muted-foreground" />
          )}
        </div>
        <div className="flex flex-wrap gap-1.5">
          {part.isOriginal && <Badge>{t('parts.original')}</Badge>}
          <StockBadge quantity={part.stockQuantity} />
        </div>
        <CardTitle className="leading-snug">{part.name}</CardTitle>
        <CardDescription>
          {part.manufacturer} · {part.sku}
        </CardDescription>
        <p className="mt-auto text-lg font-semibold">{t('parts.price', { price: formatSum(part.price) })}</p>
      </Card>
    </Link>
  )
}
