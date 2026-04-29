using CafeApi.Models;

namespace CafeApi.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetCustomerById(int id);
    
}