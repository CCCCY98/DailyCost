import { defineStore } from 'pinia'
import type { CategoryDto } from '@/types/category'
import { listCategories } from '@/api/category'

export const useCategoryStore = defineStore('category', {
  state: () => ({
    items: [] as CategoryDto[],
    loaded: false,
  }),
  actions: {
    async ensureLoaded() {
      if (this.loaded) return
      await this.reload()
    },
    async reload() {
      this.items = await listCategories()
      this.loaded = true
    },
  },
})

