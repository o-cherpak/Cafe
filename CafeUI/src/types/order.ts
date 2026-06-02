import type {OrderStatus} from "./enums/enums.ts";

export interface CreateOrderDto {
  customerId: number
  items: OrderItemDto[]
}

export interface OrderItemDto {
  menuItemId: number
  quantity: number
}

export interface OrderItemResponseDto {
  menuItemName: string
  quantity: number
  unitPrice: number
}

export interface OrderResponseDto {
  id: number
  customerName: string
  customerId: number
  status: keyof typeof OrderStatus
  createdAt: string
  total: number
  finalTotal: number
  items: OrderItemResponseDto[]
}