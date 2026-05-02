using CafeApi.DTOs;
using CafeApi.Enums;

namespace CafeApi.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetAllAsync(int? customerId);
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
}