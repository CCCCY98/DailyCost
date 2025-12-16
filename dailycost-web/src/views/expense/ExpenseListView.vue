<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import AppShell from '@/components/common/AppShell.vue'
import { deleteExpense, listExpenses, updateExpenseStatus } from '@/api/expense'
import type { ExpenseDto, ExpenseStatus } from '@/types/expense'
import { formatMoney } from '@/utils/format'
import { useAuthStore } from '@/stores/auth'
import { useCategoryStore } from '@/stores/category'

const auth = useAuthStore()
const categories = useCategoryStore()

const loading = ref(false)
const rows = ref<ExpenseDto[]>([])
const total = ref(0)
const query = reactive({
  keyword: '',
  status: 0 as ExpenseStatus,
  categoryId: '' as string,
  page: 1,
  pageSize: 20,
})

async function load() {
  loading.value = true
  try {
    await categories.ensureLoaded()
    const res = await listExpenses({
      keyword: query.keyword || undefined,
      status: query.status,
      categoryId: query.categoryId || undefined,
      page: query.page,
      pageSize: query.pageSize,
      sortBy: 'dailyCost',
      sortOrder: 'desc',
    })
    rows.value = res.items
    total.value = res.total
  } finally {
    loading.value = false
  }
}

async function toggleStatus(row: ExpenseDto) {
  const next: ExpenseStatus = row.status === 0 ? 1 : 0
  await updateExpenseStatus(row.id, next)
  await load()
}

async function remove(row: ExpenseDto) {
  await deleteExpense(row.id)
  await load()
}

onMounted(load)
</script>

<template>
  <AppShell>
    <el-card>
      <div class="toolbar">
        <el-input v-model="query.keyword" placeholder="搜索名称" style="max-width: 240px" @keyup.enter="load" />
        <el-select v-model="query.categoryId" placeholder="分类" clearable style="width: 180px" @change="load">
          <el-option v-for="c in categories.items" :key="c.id" :label="c.name" :value="c.id" />
        </el-select>
        <el-segmented v-model="query.status" :options="[
          { label: '使用中', value: 0 },
          { label: '已停用', value: 1 },
        ]" @change="load" />
        <div class="spacer" />
        <el-button type="primary" @click="$router.push('/expenses/new')">新增</el-button>
      </div>
    </el-card>

    <el-card style="margin-top: 12px">
      <el-table :data="rows" v-loading="loading" style="width: 100%">
        <el-table-column prop="name" label="名称" min-width="220" />
        <el-table-column label="分类" min-width="140">
          <template #default="{ row }">{{ row.category?.name ?? '未分类' }}</template>
        </el-table-column>
        <el-table-column prop="expenseTypeName" label="类型" width="120" />
        <el-table-column label="日均成本" width="140">
          <template #default="{ row }">{{ formatMoney(row.dailyCost, auth.user?.currency ?? 'CNY') }}</template>
        </el-table-column>
        <el-table-column prop="usedDays" label="已使用(天)" width="110" />
        <el-table-column width="200" label="操作">
          <template #default="{ row }">
            <el-space>
              <el-link @click="$router.push(`/expenses/${row.id}/edit`)">编辑</el-link>
              <el-link @click="toggleStatus(row)">{{ row.status === 0 ? '停用' : '启用' }}</el-link>
              <el-popconfirm title="确定删除？（软删除）" @confirm="remove(row)">
                <template #reference>
                  <el-link type="danger">删除</el-link>
                </template>
              </el-popconfirm>
            </el-space>
          </template>
        </el-table-column>
      </el-table>

      <div class="pager">
        <el-pagination
          layout="prev, pager, next, sizes, total"
          :total="total"
          v-model:current-page="query.page"
          v-model:page-size="query.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          @change="load"
        />
      </div>
    </el-card>
  </AppShell>
</template>

<style scoped>
.toolbar {
  display: flex;
  gap: 10px;
  align-items: center;
  flex-wrap: wrap;
}
.spacer {
  flex: 1;
}
.pager {
  margin-top: 12px;
  display: flex;
  justify-content: flex-end;
}
</style>

