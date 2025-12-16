import axios from 'axios'
import type { ApiResponse } from '@/types/api'
import { storage } from '@/utils/storage'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE ?? '/api/v1',
  timeout: 15000,
})

api.interceptors.request.use((config) => {
  const token = storage.get<string>('accessToken')
  if (token) {
    config.headers = config.headers ?? {}
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (resp) => resp,
  (err) => {
    if (err?.response?.status === 401) {
      storage.remove('accessToken')
      storage.remove('refreshToken')
      storage.remove('user')
    }
    return Promise.reject(err)
  },
)

export async function unwrap<T>(p: Promise<{ data: ApiResponse<T> }>): Promise<T> {
  try {
    const { data } = await p
    if (!data.success) throw new Error(data.message ?? '请求失败')
    if (data.data == null) throw new Error('空响应')
    return data.data
  } catch (err: any) {
    if (axios.isAxiosError(err)) {
      const msg =
        (err.response?.data as any)?.message ||
        err.response?.statusText ||
        err.message ||
        '请求失败'
      throw new Error(msg)
    }
    throw err
  }
}

export default api
