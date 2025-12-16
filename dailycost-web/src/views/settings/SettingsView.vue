<script setup lang="ts">
import { reactive, ref } from 'vue'
import AppShell from '@/components/common/AppShell.vue'
import { changePassword, updateSettings } from '@/api/user'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const saving = ref(false)

const settings = reactive({
  defaultCalcMode: auth.user?.defaultCalcMode ?? 0,
  currency: auth.user?.currency ?? 'CNY',
  timezone: auth.user?.timezone ?? 'Asia/Shanghai',
})

const pwd = reactive({
  currentPassword: '',
  newPassword: '',
})

async function saveSettings() {
  saving.value = true
  try {
    const u = await updateSettings(settings)
    auth.user = u
  } finally {
    saving.value = false
  }
}

async function savePassword() {
  saving.value = true
  try {
    await changePassword(pwd)
    pwd.currentPassword = ''
    pwd.newPassword = ''
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <AppShell>
    <el-space direction="vertical" fill>
      <el-card>
        <template #header>偏好设置</template>
        <el-form label-position="top" :model="settings">
          <el-form-item label="默认计算模式">
            <el-select v-model="settings.defaultCalcMode" style="width: 240px">
              <el-option label="动态摊销" :value="0" />
              <el-option label="固定摊销" :value="1" />
            </el-select>
          </el-form-item>
          <el-form-item label="货币">
            <el-input v-model="settings.currency" style="width: 240px" />
          </el-form-item>
          <el-form-item label="时区">
            <el-input v-model="settings.timezone" style="width: 240px" />
          </el-form-item>
          <el-button type="primary" :loading="saving" @click="saveSettings">保存</el-button>
        </el-form>
      </el-card>

      <el-card>
        <template #header>修改密码</template>
        <el-form label-position="top" :model="pwd">
          <el-form-item label="当前密码">
            <el-input v-model="pwd.currentPassword" type="password" show-password />
          </el-form-item>
          <el-form-item label="新密码">
            <el-input v-model="pwd.newPassword" type="password" show-password />
          </el-form-item>
          <el-button :loading="saving" @click="savePassword">修改</el-button>
        </el-form>
      </el-card>
    </el-space>
  </AppShell>
</template>

