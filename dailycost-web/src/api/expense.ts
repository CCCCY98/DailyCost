import api, { unwrap } from './index'
import type { ExpenseDto, ExpenseStatus } from '@/types/expense'
import type { PagedResult } from '@/types/api'

export interface ExpenseListQuery {
  status?: ExpenseStatus
  categoryId?: string
  keyword?: string
  page?: number
  pageSize?: number
  sortBy?: string
  sortOrder?: string
}

export function listExpenses(query: ExpenseListQuery) {
  return unwrap<PagedResult<ExpenseDto>>(api.get('/expenses', { params: query }))
}

export function getExpense(id: string) {
  return unwrap<ExpenseDto>(api.get(`/expenses/${id}`))
}

export function createExpense(payload: any) {
  return unwrap<ExpenseDto>(api.post('/expenses', payload))
}

export function updateExpense(id: string, payload: any) {
  return unwrap<ExpenseDto>(api.put(`/expenses/${id}`, payload))
}

export function deleteExpense(id: string) {
  return unwrap<Record<string, never>>(api.delete(`/expenses/${id}`))
}

export function updateExpenseStatus(id: string, status: ExpenseStatus) {
  return unwrap<ExpenseDto>(api.put(`/expenses/${id}/status`, { status }))
}

