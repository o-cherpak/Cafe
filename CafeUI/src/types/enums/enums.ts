export const ItemCategory = {
  Beverages: 0,
  Food: 1,
} as const;
export type ItemCategoryType = typeof ItemCategory[keyof typeof ItemCategory];


export const OrderStatus = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
  Cancelled: 3,
} as const;
export type OrderStatusType = typeof OrderStatus[keyof typeof OrderStatus];


export const DiscountType = {
  Percentage: 0,
  FixedAmount: 1,
} as const;
export type DiscountTypeType = typeof DiscountType[keyof typeof DiscountType];

export const UserRole = {
  Customer: 0,
  Barista: 1,
  Admin: 2,
} as const;
export type UserRoleType = typeof UserRole[keyof typeof UserRole];