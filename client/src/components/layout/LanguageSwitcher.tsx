import { useTranslation } from 'react-i18next'

import { Button } from '@/components/ui/button'
import { languages } from '@/i18n'

export function LanguageSwitcher() {
  const { i18n } = useTranslation()
  return (
    <div className="flex gap-1">
      {languages.map((lng) => (
        <Button
          key={lng}
          size="sm"
          variant={i18n.resolvedLanguage === lng ? 'secondary' : 'ghost'}
          onClick={() => void i18n.changeLanguage(lng)}
        >
          {lng.toUpperCase()}
        </Button>
      ))}
    </div>
  )
}
