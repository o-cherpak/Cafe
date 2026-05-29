import type {UserRole} from "./enums/enums.ts";

export interface RegisterDto {
  name?: string
  email: string
  password: string
  role: typeof UserRole
}

export interface LoginDto {
  email: string
  password: string
}

export interface AuthResponseDto {
  token: string
  email: string
  role: typeof UserRole
  customerId?: number
}