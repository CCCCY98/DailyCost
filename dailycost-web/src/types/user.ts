export type CalcMode = 0 | 1

export interface UserDto {
  id: string
  email: string
  nickname?: string | null
  avatar?: string | null
  defaultCalcMode: CalcMode
  currency: string
  timezone: string
  createdAt: string
  updatedAt: string
  lastLoginAt?: string | null
}

