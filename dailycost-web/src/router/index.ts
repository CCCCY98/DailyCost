import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  // 部署在 /dailycost 下，需指定 base；开发环境不受影响
  history: createWebHistory('/dailycost'),
  routes: [
    { path: '/login', component: () => import('@/views/auth/LoginView.vue'), meta: { public: true } },
    { path: '/register', component: () => import('@/views/auth/RegisterView.vue'), meta: { public: true } },
    { path: '/', component: () => import('@/views/home/HomeView.vue') },
    { path: '/expenses', component: () => import('@/views/expense/ExpenseListView.vue') },
    { path: '/expenses/new', component: () => import('@/views/expense/ExpenseFormView.vue') },
    { path: '/expenses/:id/edit', component: () => import('@/views/expense/ExpenseFormView.vue') },
    { path: '/statistics', component: () => import('@/views/statistics/StatisticsView.vue') },
    { path: '/settings', component: () => import('@/views/settings/SettingsView.vue') },
  ],
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()
  if (to.meta.public) return true
  if (auth.isAuthed) return true
  return { path: '/login', query: { redirect: to.fullPath } }
})

export default router
