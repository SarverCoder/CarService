import { Clock } from 'lucide-react'
import { useTranslation } from 'react-i18next'

import { Card, CardDescription, CardTitle } from '@/components/ui/card'
import { useServices } from '@/features/services/api'
import { formatSum } from '@/lib/utils'

export function ServicesPage() {
  const { t, i18n } = useTranslation()
  const { data, isPending, isError } = useServices()

  return (
    <section className="flex flex-col gap-6">
      <h1 className="text-3xl font-bold tracking-tight">{t('services.title')}</h1>
      {isPending && <p className="text-muted-foreground">{t('common.loading')}</p>}
      {isError && <p className="text-destructive">{t('services.error')}</p>}
      {data?.length === 0 && <p className="text-muted-foreground">{t('services.empty')}</p>}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {data?.map((service) => (
          <Card key={service.id}>
            <CardTitle>{i18n.resolvedLanguage === 'ru' ? service.nameRu : service.nameUz}</CardTitle>
            {service.description && <CardDescription>{service.description}</CardDescription>}
            <div className="mt-auto flex items-center justify-between text-sm">
              <span className="flex items-center gap-1 text-muted-foreground">
                <Clock className="size-4" />
                {t('services.duration', { minutes: service.durationMinutes })}
              </span>
              <span className="font-medium">
                {t('services.price', { from: formatSum(service.priceFrom), to: formatSum(service.priceTo) })}
              </span>
            </div>
          </Card>
        ))}
      </div>
    </section>
  )
}
