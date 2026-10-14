import type { ComponentProps } from 'react'

import { cn } from '@/lib/utils'

export function Card({ className, ...props }: ComponentProps<'div'>) {
  return <div className={cn('flex flex-col gap-4 rounded-xl border bg-card p-6 text-card-foreground shadow-sm', className)} {...props} />
}

export function CardTitle({ className, ...props }: ComponentProps<'h3'>) {
  return <h3 className={cn('font-semibold leading-none', className)} {...props} />
}

export function CardDescription({ className, ...props }: ComponentProps<'p'>) {
  return <p className={cn('text-sm text-muted-foreground', className)} {...props} />
}
