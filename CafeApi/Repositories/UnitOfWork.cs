using CafeApi.Data;
using CafeApi.Interfaces;

namespace CafeApi.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CafeDbContext _db;

    public IMenuItemRepository MenuItems { get; }
    public ICustomerRepository Customers { get; }
    public IOrderRepository Orders { get; }

    public UnitOfWork(CafeDbContext db)
    {
        _db = db;
        MenuItems = new MenuRepository(_db);
        Customers = new CustomerRepository(_db);
        Orders = new OrderRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}