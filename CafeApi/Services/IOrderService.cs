using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Models;

namespace CafeApi.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
}