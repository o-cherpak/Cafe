using CafeApi.DTOs;
using CafeApi.Interfaces;
using CafeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public CustomerController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _uow.Customers.GetAllAsync();

        var result = customers.Select(customer =>
            new CustomerDto(customer.Id, customer.Name, customer.Email, customer.BonusPoints)
        );

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _uow.Customers.GetByIdAsync(id);

        if (customer is null) return NotFound();

        var dto = new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.BonusPoints
        );

        return Ok(dto);
    }

    [HttpGet("by-email")]
    public async Task<ActionResult<CustomerDto>> GetByEmail([FromQuery] string email)
    {
        var customer = await _uow.Customers.GetCustomerByEmailAsync(email);

        if (customer is null) return NotFound();

        var dto = new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.BonusPoints
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto)
    {
        var newCustomer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            BonusPoints = 0,
            RegisteredAt = DateTime.UtcNow,
        };

        await _uow.Customers.AddAsync(newCustomer);
        await _uow.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = newCustomer.Id },
            new CustomerDto(
                newCustomer.Id,
                newCustomer.Name,
                newCustomer.Email,
                newCustomer.BonusPoints
            )
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCustomerDto dto)
    {
        var item = await _uow.Customers.GetByIdAsync(id);

        if (item is null) return NotFound();

        if (dto.Name is not null) item.Name = dto.Name;
        if (dto.Email is not null) item.Email = dto.Email;

        await _uow.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var item = await _uow.Customers.GetByIdAsync(id);

        if (item is null) return NotFound();

        _uow.Customers.Delete(item);
        await _uow.SaveChangesAsync();

        return NoContent();
    }
}