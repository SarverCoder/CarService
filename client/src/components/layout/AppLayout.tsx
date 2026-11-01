import { Wrench } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { Link, NavLink, Outlet } from 'react-router'

import { cn } from '@/lib/utils'

import { LanguageSwitcher } from './LanguageSwitcher'

const navItems = [
  { to: '/', key: 'nav.home', end: true },
  { to: '/services', key: 'nav.services' },
  { to: '/parts', key: 'nav.parts' },
] as const

const currentYear = new Date().getFullYear()

export function AppLayout() {
  const { t } = useTranslation()
  return (
    <div className="flex min-h-svh flex-col">
      <header className="border-b">
        <div className="mx-auto flex h-16 max-w-6xl items-center gap-6 px-4">
          <Link to="/" className="flex items-center gap-2 font-semibold">
            <Wrench className="size-5 text-primary" />
            {t('brand')}
          </Link>
          <nav className="flex flex-1 gap-4 text-sm">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                end={'end' in item}
                className={({ isActive }) =>
                  cn('text-muted-foreground hover:text-foreground', isActive && 'font-medium text-foreground')
                }
              >
                {t(item.key)}
              </NavLink>
            ))}
          </nav>
          <LanguageSwitcher />
        </div>
      </header>
      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
        <Outlet />
      </main>
      <footer className="border-t py-6 text-center text-sm text-muted-foreground">
        {t('footer', { year: currentYear })}
      </footer>
    </div>
  )
}
