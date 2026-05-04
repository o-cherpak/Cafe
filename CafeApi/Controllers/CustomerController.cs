using CafeApi.DTOs;
using CafeApi.Interfaces;
using CafeApi.Services.CustomerService;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var listDto = await _customerService.GetAll();

        return Ok(listDto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var dto = await _customerService.GetById(id);

        return Ok(dto);
    }

    [HttpGet("by-email")]
    public async Task<ActionResult<CustomerDto>> GetByEmail([FromQuery] string email)
    {
        var dto = await _customerService.GetByEmail(email);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto)
    {
        var responseDto = await _customerService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = responseDto.Id },
            responseDto
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCustomerDto dto)
    {
        await _customerService.Update(id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _customerService.Delete(id);

        return NoContent();
    }
}