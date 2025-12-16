import api, { unwrap } from './index'
import type { CategoryDto } from '@/types/category'

export function listCategories() {
  return unwrap<CategoryDto[]>(api.get('/categories'))
}

