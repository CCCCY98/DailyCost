<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import dayjs from 'dayjs'
import AppShell from '@/components/common/AppShell.vue'
import { createExpense, getExpense, updateExpense } from '@/api/expense'
import { useCategoryStore } from '@/stores/category'
import type { ExpenseType } from '@/types/expense'

const route = useRoute()
const router = useRouter()
const categories = useCategoryStore()

const id = computed(() => route.params.id as string | undefined)
const isEdit = computed(() => !!id.value)
const loading = ref(false)

const form = reactive({
  name: '',
  amount: 0,
  expenseType: 0 as ExpenseType,
  categoryId: '' as string,
  startDate: dayjs().format('YYYY-MM-DD'),
  expectedDays: undefined as number | undefined,
  billingCycle: 0 as 0 | 1 | 2,
  calcMode: null as null | 0 | 1,
  autoRenew: true,
  note: '',
  tagsText: '',
})

const showExpectedDays = computed(() => form.expenseType === 0 && form.calcMode === 1)
const showBilling = computed(() => form.expenseType !== 0)

async function load() {
  loading.value = true
  try {
    await categories.ensureLoaded()
    if (!id.value) return
    const e = await getExpense(id.value)
    form.name = e.name
    form.amount = e.amount
    form.expenseType = e.expenseType
    form.categoryId = e.category?.id ?? ''
    form.startDate = dayjs(e.startDate).format('YYYY-MM-DD')
    form.expectedDays = e.expectedDays ?? undefined
    form.billingCycle = (e.billingCycle ?? 0) as any
    form.calcMode = (e.calcMode ?? null) as any
    form.autoRenew = e.autoRenew
    form.note = e.note ?? ''
    form.tagsText = (e.tags ?? []).join(', ')
  } finally {
    loading.value = false
  }
}

onMounted(load)

async function submit() {
  loading.value = true
  try {
    const tags = form.tagsText
      .split(',')
      .map((s) => s.trim())
      .filter(Boolean)
    const payload: any = {
      name: form.name,
      amount: form.amount,
      expenseType: form.expenseType,
      categoryId: form.categoryId || null,
      startDate: form.startDate,
      expectedDays: showExpectedDays.value ? form.expectedDays ?? null : null,
      billingCycle: showBilling.value ? form.billingCycle : null,
      calcMode: form.calcMode,
      autoRenew: showBilling.value ? form.autoRenew : true,
      note: form.note || null,
      tags,
    }

    if (id.value) await updateExpense(id.value, payload)
    else await createExpense(payload)
    router.push('/expenses')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <AppShell>
    <el-card>
      <template #header>
        <div class="header">
          <span>{{ isEdit ? '编辑消费项' : '新增消费项' }}</span>
          <el-link @click="$router.back()">返回</el-link>
        </div>
      </template>

      <el-form label-position="top" :model="form" v-loading="loading" @submit.prevent="submit">
        <el-form-item label="名称">
          <el-input v-model="form.name" />
        </el-form-item>
        <el-form-item label="金额">
          <el-input-number v-model="form.amount" :min="0" :precision="2" :step="10" style="width: 240px" />
        </el-form-item>
        <el-form-item label="消费类型">
          <el-select v-model="form.expenseType" style="width: 240px">
            <el-option label="固定资产" :value="0" />
            <el-option label="订阅服务" :value="1" />
            <el-option label="周期支出" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="分类（可选）">
          <el-select v-model="form.categoryId" clearable style="width: 240px">
            <el-option v-for="c in categories.items" :key="c.id" :label="c.name" :value="c.id" />
          </el-select>
        </el-form-item>

        <el-form-item label="开始日期">
          <el-date-picker v-model="form.startDate" type="date" value-format="YYYY-MM-DD" style="width: 240px" />
        </el-form-item>

        <el-form-item v-if="form.expenseType === 0" label="计算模式（可选，留空跟随用户设置）">
          <el-select v-model="form.calcMode" clearable style="width: 240px">
            <el-option label="动态摊销" :value="0" />
            <el-option label="固定摊销" :value="1" />
          </el-select>
        </el-form-item>

        <el-form-item v-if="showExpectedDays" label="预计使用天数">
          <el-input-number v-model="form.expectedDays" :min="1" :step="30" style="width: 240px" />
        </el-form-item>

        <el-form-item v-if="showBilling" label="订阅/周期">
          <el-select v-model="form.billingCycle" style="width: 240px">
            <el-option label="月" :value="0" />
            <el-option label="季" :value="1" />
            <el-option label="年" :value="2" />
          </el-select>
        </el-form-item>

        <el-form-item v-if="showBilling" label="自动续费">
          <el-switch v-model="form.autoRenew" />
        </el-form-item>

        <el-form-item label="标签（逗号分隔，可选）">
          <el-input v-model="form.tagsText" placeholder="例如：工作, 学习" />
        </el-form-item>

        <el-form-item label="备注（可选）">
          <el-input v-model="form.note" type="textarea" :rows="3" />
        </el-form-item>

        <el-button type="primary" :loading="loading" @click="submit">保存</el-button>
      </el-form>
    </el-card>
  </AppShell>
</template>

<style scoped>
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
