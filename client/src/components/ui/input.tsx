import type { ComponentProps } from 'react'

import { cn } from '@/lib/utils'

const fieldClass =
  'h-9 w-full min-w-0 rounded-md border border-input bg-background px-3 text-sm shadow-xs outline-none transition-[color,box-shadow] focus-visible:ring-[3px] focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50'

export function Input({ className, ...props }: ComponentProps<'input'>) {
  return <input className={cn(fieldClass, 'placeholder:text-muted-foreground', className)} {...props} />
}

export function NativeSelect({ className, ...props }: ComponentProps<'select'>) {
  return <select className={cn(fieldClass, 'pr-8', className)} {...props} />
}

export function Label({ className, ...props }: ComponentProps<'label'>) {
  return <label className={cn('flex flex-col gap-1.5 text-sm font-medium', className)} {...props} />
}
