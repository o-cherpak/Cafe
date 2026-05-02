using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Models;

namespace CafeApi.Services;

public interface IOrderService
{
    OrderResponseDto ToDto(Order order);
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
}