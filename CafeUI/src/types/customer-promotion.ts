import type {PromotionDto} from "./promotions.ts";

export interface CustomerPromotionDto {
  id: number
  customerId: number
  promotion: PromotionDto
  isUsed: boolean
  purchasedAt: string
  usedAt?: string
  usedInOrderId?: number
}

export interface BuyPromotionDto {
  customerId: number
  promotionId: number
}