export type ApiErrorMap = Record<string, string[]>

export interface ApiResponse<T> {
  success: boolean
  data: T | null
  message: string | null
  errors?: ApiErrorMap
}

export interface PagedResult<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

