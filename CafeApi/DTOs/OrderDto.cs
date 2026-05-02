using CafeApi.Enums;

namespace CafeApi.DTOs;

public record CreateOrderDto(int CustomerId, List<OrderItemDto> Items);

public record OrderResponseDto(
    int Id,
    string CustomerName,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal Total,
    List<OrderItemResponseDto> Items);

public record OrderItemDto(int MenuItemId, int Quantity);

public record OrderItemResponseDto(string MenuItemName, int Quantity, decimal UnitPrice);