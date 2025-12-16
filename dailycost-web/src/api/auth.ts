import api, { unwrap } from './index'
import type { UserDto } from '@/types/user'

export interface AuthTokensDto {
  accessToken: string
  refreshToken: string
  expiresAtUtc: string
}

export interface AuthResponseDto {
  tokens: AuthTokensDto
  user: UserDto
}

export function register(email: string, password: string, nickname?: string) {
  return unwrap<AuthResponseDto>(api.post('/auth/register', { email, password, nickname }))
}

export function login(email: string, password: string) {
  return unwrap<AuthResponseDto>(api.post('/auth/login', { email, password }))
}

export function refreshToken(refreshToken: string) {
  return unwrap<AuthTokensDto>(api.post('/auth/refresh-token', { refreshToken }))
}

export function logout(refreshToken: string) {
  return unwrap<Record<string, never>>(api.post('/auth/logout', { refreshToken }))
}

