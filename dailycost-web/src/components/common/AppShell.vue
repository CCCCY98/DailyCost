<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const active = computed(() => {
  const p = route.path
  if (p.startsWith('/expenses')) return '/expenses'
  if (p.startsWith('/statistics')) return '/statistics'
  if (p.startsWith('/settings')) return '/settings'
  return '/'
})

async function doLogout() {
  await auth.logout()
  router.replace('/login')
}
</script>

<template>
  <el-container style="min-height: 100vh">
    <el-header height="56px" class="header">
      <div class="brand" @click="$router.push('/')">DailyCost</div>
      <div class="right">
        <span class="user">{{ auth.user?.nickname || auth.user?.email }}</span>
        <el-button size="small" @click="doLogout">退出</el-button>
      </div>
    </el-header>
    <el-container>
      <el-aside width="200px" class="aside">
        <el-menu :default-active="active" router>
          <el-menu-item index="/">首页</el-menu-item>
          <el-menu-item index="/expenses">消费项</el-menu-item>
          <el-menu-item index="/statistics">统计</el-menu-item>
          <el-menu-item index="/settings">设置</el-menu-item>
        </el-menu>
      </el-aside>
      <el-main class="main">
        <slot />
      </el-main>
    </el-container>
  </el-container>
</template>

<style scoped>
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: white;
  border-bottom: 1px solid #e5e7eb;
}
.brand {
  font-weight: 700;
  cursor: pointer;
}
.right {
  display: flex;
  align-items: center;
  gap: 12px;
}
.user {
  color: #374151;
}
.aside {
  background: white;
  border-right: 1px solid #e5e7eb;
}
.main {
  padding: 16px;
}
@media (max-width: 768px) {
  .aside {
    display: none;
  }
}
</style>

