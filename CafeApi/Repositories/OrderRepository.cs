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

    public override async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await Db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.MenuItem)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetWithItemsAsync(int id)
    {
        return await Db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetOrderByCustomerIdAsync(int customerId)
    {
        return await Db.Orders.Where(order => order.CustomerId == customerId)
            .ToListAsync();
    }
}