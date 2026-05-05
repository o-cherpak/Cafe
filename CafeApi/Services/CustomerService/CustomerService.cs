using CafeApi.DTOs;
using CafeApi.Exceptions;
using CafeApi.Interfaces;
using CafeApi.Models;

namespace CafeApi.Services.CustomerService;

public class CustomerService : ICustomerService
{
    private IUnitOfWork _uow;

    public CustomerService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private CustomerDto ToDto(Customer customer)
    {
        var dto = new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.BonusPoints
        );

        return dto;
    }

    public async Task<IEnumerable<CustomerDto>> GetAll()
    {
        var customers = await _uow.Customers.GetAllAsync();

        var result = customers.Select(ToDto);

        return result;
    }

    public async Task<CustomerDto> GetById(int id)
    {
        var customer = await _uow.Customers.GetByIdAsync(id);

        if (customer is null)
            throw new KeyNotFoundException("Customer not found");

        return ToDto(customer);
    }

    public async Task<CustomerDto> GetByEmail(string email)
    {
        var customer = await _uow.Customers.GetCustomerByEmailAsync(email);

        if (customer is null)
            throw new KeyNotFoundException("Customer not found");

        return ToDto(customer);
    }

    public async Task<CustomerDto> Create(CreateCustomerDto dto)
    {
        var customer = await _uow.Customers.GetCustomerByEmailAsync(dto.Email);

        if (customer is not null)
            throw new ConflictException("Customer with this email already exists");

        var newCustomer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            BonusPoints = 0,
            RegisteredAt = DateTime.UtcNow,
        };

        await _uow.Customers.AddAsync(newCustomer);
        await _uow.SaveChangesAsync();

        return ToDto(newCustomer);
    }

    public async Task Update(int id, UpdateCustomerDto dto)
    {
        var item = await _uow.Customers.GetByIdAsync(id);

        if (item is null)
            throw new KeyNotFoundException("Customer not found");

        if (dto.Name is not null) item.Name = dto.Name;
        if (dto.Email is not null) item.Email = dto.Email;

        await _uow.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var item = await _uow.Customers.GetByIdAsync(id);

        if (item is null)
            throw new KeyNotFoundException("Customer not found");

        _uow.Customers.Delete(item);
        await _uow.SaveChangesAsync();
    }
}