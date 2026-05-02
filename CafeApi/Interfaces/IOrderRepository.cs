using CafeApi.Models;

namespace CafeApi.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrderByCustomerIdAsync(int customerId);
    Task<Order?> GetWithItemsAsync(int id);
}