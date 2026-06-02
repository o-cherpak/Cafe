import {ItemCategory} from "./enums/enums"

export interface MenuItemDto {
  id: number
  name: string
  category: keyof typeof ItemCategory
  price: number
  isAvailable: boolean
}

export interface CreateMenuItemDto {
  name: string
  category: keyof typeof ItemCategory
  price: number
  description?: string
}

export interface UpdateMenuItemDto {
  name?: string
  price?: number
  isAvailable?: boolean
}