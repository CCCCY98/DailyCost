<script setup lang="ts">
import { onMounted, ref } from 'vue'
import * as echarts from 'echarts'
import AppShell from '@/components/common/AppShell.vue'
import { getByCategory, getTrend } from '@/api/statistics'
import { formatMoney } from '@/utils/format'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const loading = ref(false)
const trendEl = ref<HTMLElement | null>(null)
let chart: echarts.ECharts | null = null

const byCategory = ref<{ categoryName: string; totalDailyCost: number; percent: number }[]>([])

async function load() {
  loading.value = true
  try {
    const [trend, cat] = await Promise.all([getTrend(30), getByCategory()])
    byCategory.value = cat.items

    if (trendEl.value) {
      chart?.dispose()
      chart = echarts.init(trendEl.value)
      chart.setOption({
        tooltip: { trigger: 'axis' },
        xAxis: { type: 'category', data: trend.items.map((i) => i.date.slice(0, 10)) },
        yAxis: { type: 'value' },
        series: [
          {
            type: 'line',
            data: trend.items.map((i) => i.totalDailyCost),
            smooth: true,
            areaStyle: {},
          },
        ],
      })
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  load()
  window.addEventListener('resize', () => chart?.resize())
})
</script>

<template>
  <AppShell>
    <el-space direction="vertical" fill>
      <el-card>
        <template #header>
          <div class="header">
            <span>近30天趋势</span>
            <el-button size="small" :loading="loading" @click="load">刷新</el-button>
          </div>
        </template>
        <div ref="trendEl" style="height: 320px; width: 100%" />
      </el-card>

      <el-card>
        <template #header>
          <div class="header">
            <span>分类占比（今日）</span>
          </div>
        </template>
        <el-table :data="byCategory" v-loading="loading" style="width: 100%">
          <el-table-column prop="categoryName" label="分类" min-width="180" />
          <el-table-column label="日均成本" width="160">
            <template #default="{ row }">{{ formatMoney(row.totalDailyCost, auth.user?.currency ?? 'CNY') }}</template>
          </el-table-column>
          <el-table-column prop="percent" label="占比(%)" width="120" />
        </el-table>
      </el-card>
    </el-space>
  </AppShell>
</template>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>

