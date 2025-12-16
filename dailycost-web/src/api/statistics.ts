import api, { unwrap } from './index'

export interface TodayResponseDto {
  date: string
  totalDailyCost: number
  activeCount: number
  currency: string
}

export interface TrendPointDto {
  date: string
  totalDailyCost: number
}

export interface TrendResponseDto {
  days: number
  items: TrendPointDto[]
}

export interface ByCategoryItemDto {
  categoryId?: string | null
  categoryName: string
  totalDailyCost: number
  percent: number
}

export interface ByCategoryResponseDto {
  date: string
  totalDailyCost: number
  items: ByCategoryItemDto[]
}

export function getToday() {
  return unwrap<TodayResponseDto>(api.get('/statistics/today'))
}

export function getTrend(days = 30) {
  return unwrap<TrendResponseDto>(api.get('/statistics/trend', { params: { days } }))
}

export function getByCategory() {
  return unwrap<ByCategoryResponseDto>(api.get('/statistics/by-category'))
}

