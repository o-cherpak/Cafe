using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Models;

namespace CafeApi.Services;

public interface IOrderService
{
    OrderResponseDto ToDto(Order order);
    Task<IEnumerable<OrderResponseDto>> GetAllAsync(int? customerId);
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
}