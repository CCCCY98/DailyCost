export interface CategoryDto {
  id: string
  userId?: string | null
  name: string
  icon?: string | null
  color?: string | null
  isSystem: boolean
  sortOrder: number
}

