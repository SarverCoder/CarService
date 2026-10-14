import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'

import { Button } from '@/components/ui/button'

export function NotFoundPage() {
  const { t } = useTranslation()
  return (
    <section className="flex flex-col items-center gap-4 py-20 text-center">
      <h1 className="text-3xl font-bold">404</h1>
      <p className="text-muted-foreground">{t('notFound.title')}</p>
      <Button asChild variant="outline">
        <Link to="/">{t('notFound.back')}</Link>
      </Button>
    </section>
  )
}
