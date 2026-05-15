using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = "Admin,Barista")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] OrderStatus? status
    )
    {
        var listDto = await _orderService.GetAll(customerId, status);
        return Ok(listDto);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var dto = await _orderService.GetById(id);

        return Ok(dto);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpGet("by-customer")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderByCustomerId([FromQuery] int customerId)
    {
        var dto = await _orderService.GetOrderByCustomerId(customerId);

        return Ok(dto);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create(CreateOrderDto dto)
    {
        var result = await _orderService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Admin,Barista")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] OrderStatus status)
    {
        await _orderService.Update(id, status);

        return NoContent();
    }
}