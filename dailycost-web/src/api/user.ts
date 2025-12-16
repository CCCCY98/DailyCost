import api, { unwrap } from './index'
import type { UserDto } from '@/types/user'

export function getMe() {
  return unwrap<UserDto>(api.get('/users/me'))
}

export function updateMe(payload: { nickname?: string | null; avatar?: string | null }) {
  return unwrap<UserDto>(api.put('/users/me', payload))
}

export function updateSettings(payload: { defaultCalcMode: number; currency: string; timezone: string }) {
  return unwrap<UserDto>(api.put('/users/me/settings', payload))
}

export function changePassword(payload: { currentPassword: string; newPassword: string }) {
  return unwrap<Record<string, never>>(api.put('/users/me/password', payload))
}

