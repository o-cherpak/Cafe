import type {DiscountType} from "./enums/enums.ts";

export interface PromotionDto {
  id: number
  name: string
  description?: string
  bonusCost: number
  discountType: keyof typeof DiscountType
  discountValue: number
  isActive: boolean
}

export interface CreatePromotionDto {
  name: string
  description?: string
  bonusCost: number
  discountType: keyof typeof DiscountType
  discountValue: number
}

export interface UpdatePromotionDto {
  name?: string
  description?: string
  discountValue?: number
  isActive?: boolean
}