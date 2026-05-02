using CafeApi.DTOs;
using CafeApi.Enums;

namespace CafeApi.Services;

public class OrderService : IOrderService
{
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