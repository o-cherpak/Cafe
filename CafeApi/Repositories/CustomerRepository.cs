using CafeApi.Data;
using CafeApi.Interfaces;
using CafeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(CafeDbContext db) : base(db)
    {
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        return await Db.Customers.FirstOrDefaultAsync(customer => customer.Email == email);
    }
}