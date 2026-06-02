export interface CustomerDto {
  id: number
  name: string
  email: string
  bonusPoints: number
}

export interface CreateCustomerDto {
  name: string
  email: string
}

export interface UpdateCustomerDto {
  name?: string
  email?: string
}