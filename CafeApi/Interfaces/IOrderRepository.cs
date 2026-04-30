using CafeApi.Enums;
using CafeApi.Models;

namespace CafeApi.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrderByStatusAsync(OrderStatus status);
    Task<IEnumerable<Order>> GetOrderByCustomerIdAsync(int customerId);
}