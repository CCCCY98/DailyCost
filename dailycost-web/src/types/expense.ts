export type ExpenseType = 0 | 1 | 2
export type ExpenseStatus = 0 | 1
export type BillingCycle = 0 | 1 | 2
export type CalcMode = 0 | 1

export interface CategoryBriefDto {
  id: string
  name: string
  icon?: string | null
  color?: string | null
}

export interface ExpenseDto {
  id: string
  name: string
  amount: number
  expenseType: ExpenseType
  expenseTypeName: string
  category?: CategoryBriefDto | null
  startDate: string
  endDate?: string | null
  usedDays: number
  expectedDays?: number | null
  billingCycle?: BillingCycle | null
  autoRenew: boolean
  nextRenewalDate?: string | null
  calcMode?: CalcMode | null
  status: ExpenseStatus
  dailyCost: number
  note?: string | null
  imageUrl?: string | null
  tags: string[]
  createdAt: string
  updatedAt: string
}

