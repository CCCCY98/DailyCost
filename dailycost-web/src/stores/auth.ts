import { defineStore } from 'pinia'
import type { UserDto } from '@/types/user'
import * as authApi from '@/api/auth'
import { storage } from '@/utils/storage'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    accessToken: storage.get<string>('accessToken'),
    refreshToken: storage.get<string>('refreshToken'),
    user: storage.get<UserDto>('user'),
  }),
  getters: {
    isAuthed: (s) => !!s.accessToken,
  },
  actions: {
    async register(email: string, password: string, nickname?: string) {
      const res = await authApi.register(email, password, nickname)
      this.setAuth(res.tokens.accessToken, res.tokens.refreshToken, res.user)
    },
    async login(email: string, password: string) {
      const res = await authApi.login(email, password)
      this.setAuth(res.tokens.accessToken, res.tokens.refreshToken, res.user)
    },
    async logout() {
      if (this.refreshToken) {
        try {
          await authApi.logout(this.refreshToken)
        } catch {
          // ignore
        }
      }
      this.clear()
    },
    async refresh() {
      if (!this.refreshToken) throw new Error('no refresh token')
      const t = await authApi.refreshToken(this.refreshToken)
      this.accessToken = t.accessToken
      storage.set('accessToken', t.accessToken)
      storage.set('refreshToken', t.refreshToken)
      this.refreshToken = t.refreshToken
    },
    setAuth(accessToken: string, refreshToken: string, user: UserDto) {
      this.accessToken = accessToken
      this.refreshToken = refreshToken
      this.user = user
      storage.set('accessToken', accessToken)
      storage.set('refreshToken', refreshToken)
      storage.set('user', user)
    },
    clear() {
      this.accessToken = null
      this.refreshToken = null
      this.user = null
      storage.remove('accessToken')
      storage.remove('refreshToken')
      storage.remove('user')
    },
  },
})

