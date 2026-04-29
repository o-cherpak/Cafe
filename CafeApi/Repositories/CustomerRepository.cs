using CafeApi.Data;
using CafeApi.Interfaces;
using CafeApi.Models;

namespace CafeApi.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(CafeDbContext db) : base(db)
    {
    }

    public async Task<Customer?> GetCustomerById(int id)
    {
        return await Db.Set<Customer>().FindAsync(id);
    }
}