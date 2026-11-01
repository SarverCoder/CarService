import { CalendarCheck, Search, Tag } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'

import { Button } from '@/components/ui/button'
import { Card, CardDescription, CardTitle } from '@/components/ui/card'

const features = [
  { key: 'prices', icon: Tag },
  { key: 'parts', icon: Search },
  { key: 'booking', icon: CalendarCheck },
] as const

export function HomePage() {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-12">
      <section className="flex flex-col items-start gap-6 py-12 md:py-20">
        <h1 className="max-w-2xl text-4xl font-bold tracking-tight md:text-5xl">{t('home.title')}</h1>
        <p className="max-w-xl text-lg text-muted-foreground">{t('home.subtitle')}</p>
        <div className="flex flex-wrap gap-3">
          <Button asChild size="lg">
            <Link to="/services">{t('home.cta')}</Link>
          </Button>
          <Button asChild size="lg" variant="outline">
            <Link to="/parts">{t('home.ctaParts')}</Link>
          </Button>
        </div>
      </section>
      <section className="grid gap-4 md:grid-cols-3">
        {features.map(({ key, icon: Icon }) => (
          <Card key={key} className="gap-3">
            <Icon className="size-6 text-primary" />
            <CardTitle>{t(`home.features.${key}.title`)}</CardTitle>
            <CardDescription>{t(`home.features.${key}.text`)}</CardDescription>
          </Card>
        ))}
      </section>
    </div>
  )
}
