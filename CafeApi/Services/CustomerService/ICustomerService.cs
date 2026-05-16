using CafeApi.DTOs;

namespace CafeApi.Services.CustomerService;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAll();
    Task<CustomerDto> GetById(int id);
    Task<CustomerDto> GetByEmail(string email);
    Task<CustomerDto> Create(CreateCustomerDto dto);
    Task Update(int id, UpdateCustomerDto dto);
    Task Delete(int id);
}