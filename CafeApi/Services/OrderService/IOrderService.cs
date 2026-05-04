using CafeApi.DTOs;
using CafeApi.Enums;

namespace CafeApi.Services.OrderService;

public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetAll
        (int? customerId, OrderStatus? status);

    Task<OrderResponseDto> GetById(int id);
    Task Update(int id, OrderStatus status);
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
}