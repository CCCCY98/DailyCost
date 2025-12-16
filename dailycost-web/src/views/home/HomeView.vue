<script setup lang="ts">
import { onMounted, ref } from 'vue'
import AppShell from '@/components/common/AppShell.vue'
import { getToday } from '@/api/statistics'
import { listExpenses } from '@/api/expense'
import type { ExpenseDto } from '@/types/expense'
import { formatMoney } from '@/utils/format'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const loading = ref(false)
const total = ref<{ totalDailyCost: number; activeCount: number; currency: string } | null>(null)
const topExpenses = ref<ExpenseDto[]>([])

async function load() {
  loading.value = true
  try {
    const t = await getToday()
    total.value = { totalDailyCost: t.totalDailyCost, activeCount: t.activeCount, currency: t.currency }
    const list = await listExpenses({ status: 0, page: 1, pageSize: 10, sortBy: 'dailyCost', sortOrder: 'desc' })
    topExpenses.value = list.items
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <AppShell>
    <el-space direction="vertical" fill>
      <el-card>
        <div class="headline">
          <div>
            <div class="label">今日总消耗</div>
            <div class="value">
              {{ total ? formatMoney(total.totalDailyCost, total.currency) : '-' }}
            </div>
            <div class="sub">
              <span>有效消费项：{{ total?.activeCount ?? '-' }}</span>
              <span class="sep">·</span>
              <span>计算模式：{{ auth.user?.defaultCalcMode === 0 ? '动态摊销' : '固定摊销' }}</span>
            </div>
          </div>
          <el-button type="primary" @click="$router.push('/expenses/new')">添加消费项</el-button>
        </div>
      </el-card>

      <el-card>
        <template #header>
          <div class="cardHeader">
            <span>消费项（按日均成本）</span>
            <el-link @click="$router.push('/expenses')">查看全部</el-link>
          </div>
        </template>
        <el-table :data="topExpenses" v-loading="loading" style="width: 100%">
          <el-table-column prop="name" label="名称" min-width="180" />
          <el-table-column label="分类" min-width="120">
            <template #default="{ row }">{{ row.category?.name ?? '未分类' }}</template>
          </el-table-column>
          <el-table-column label="日均成本" width="140">
            <template #default="{ row }">{{ formatMoney(row.dailyCost, total?.currency ?? 'CNY') }}</template>
          </el-table-column>
          <el-table-column prop="usedDays" label="已使用(天)" width="110" />
          <el-table-column width="120">
            <template #default="{ row }">
              <el-link @click="$router.push(`/expenses/${row.id}/edit`)">编辑</el-link>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </el-space>
  </AppShell>
</template>

<style scoped>
.headline {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: center;
}
.label {
  color: #6b7280;
  font-size: 14px;
}
.value {
  font-size: 32px;
  font-weight: 800;
  margin-top: 6px;
}
.sub {
  color: #6b7280;
  font-size: 13px;
  margin-top: 6px;
}
.sep {
  margin: 0 6px;
}
.cardHeader {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>

