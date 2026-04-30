using CafeApi.Data;
using CafeApi.Enums;
using CafeApi.Interfaces;
using CafeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(CafeDbContext db) : base(db)
    {
    }

    public async Task<IEnumerable<Order>> GetOrderByStatusAsync(OrderStatus status)
    {
        return await Db.Orders.Where(order => order.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrderByCustomerIdAsync(int customerId)
    {
        return await Db.Orders.Where(order => order.CustomerId == customerId)
            .ToListAsync();
    }
}