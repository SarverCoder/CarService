import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'

import { Button } from '@/components/ui/button'

export function HomePage() {
  const { t } = useTranslation()
  return (
    <section className="flex flex-col items-start gap-6 py-12 md:py-20">
      <h1 className="max-w-2xl text-4xl font-bold tracking-tight md:text-5xl">{t('home.title')}</h1>
      <p className="max-w-xl text-lg text-muted-foreground">{t('home.subtitle')}</p>
      <Button asChild size="lg">
        <Link to="/services">{t('home.cta')}</Link>
      </Button>
    </section>
  )
}
