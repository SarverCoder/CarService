import { clsx, type ClassValue } from 'clsx'
import { twMerge } from 'tailwind-merge'

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatSum(value: number) {
  return new Intl.NumberFormat('uz-UZ').format(value)
}
