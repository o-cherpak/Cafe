export const ItemCategory = {
  Beverages: 0,
  Food: 1,
} as const;

export const OrderStatus = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
  Cancelled: 3,
} as const;

export const DiscountType = {
  Percentage: 0,
  FixedAmount: 1,
} as const;

export const UserRole = {
  Customer: 0,
  Barista: 1,
  Admin: 2,
} as const;