using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Models;

namespace CafeApi.Services;

public class OrderService : IOrderService
{
    public OrderResponseDto ToDto(Order order)
    {
        var dto = new OrderResponseDto(
            order.Id, order.Customer.Name,
            order.Status,
            order.CreatedAt,
            Total: order.Items.Sum(i => i.UnitPrice * i.Quantity),
            Items: order.Items.Select(i =>
                new OrderItemResponseDto(
                    i.MenuItem.Name,
                    i.Quantity,
                    i.UnitPrice
                )
            ).ToList()
        );

        return dto;
    }

    public Task<IEnumerable<OrderResponseDto>> GetAllAsync(int? customerId)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        throw new NotImplementedException();
    }
}