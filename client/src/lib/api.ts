const TOKEN_KEY = 'carservice.token'

export const tokenStorage = {
  get: () => localStorage.getItem(TOKEN_KEY),
  set: (token: string) => localStorage.setItem(TOKEN_KEY, token),
  clear: () => localStorage.removeItem(TOKEN_KEY),
}

export interface ProblemDetails {
  title?: string
  status?: number
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  readonly status: number
  readonly problem: ProblemDetails

  constructor(status: number, problem: ProblemDetails) {
    super(problem.title ?? `HTTP ${status}`)
    this.status = status
    this.problem = problem
  }
}

const baseUrl = import.meta.env.VITE_API_URL ?? ''

/** Backend API'ga so'rov: JWT tokenni qo'shadi va ProblemDetails xatolarini ApiError'ga aylantiradi. */
export async function apiFetch<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers)
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json')
  const token = tokenStorage.get()
  if (token) headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(`${baseUrl}${path}`, { ...init, headers })
  if (!response.ok) {
    const problem = (await response.json().catch(() => ({}))) as ProblemDetails
    throw new ApiError(response.status, problem)
  }
  return response.status === 204 ? (undefined as T) : ((await response.json()) as T)
}
