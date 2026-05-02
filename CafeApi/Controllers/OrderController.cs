using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Interfaces;
using CafeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IOrderService _orderService;

    public OrderController(IUnitOfWork uof, IOrderService orderService)
    {
        _uof = uof;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] OrderStatus? status
    )
    {
        var orders = customerId.HasValue
            ? await _uof.Orders.GetOrderByCustomerIdAsync(customerId.Value)
            : await _uof.Orders.GetAllAsync();

        if (status.HasValue)
        {
            orders = orders.Where(o => o.Status == status.Value);
        }

        var result =
            orders.Select(order => _orderService.ToDto(order));

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var order = await _uof.Orders.GetWithItemsAsync(id);

        if (order is null) return NotFound();

        return Ok(_orderService.ToDto(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create(CreateOrderDto dto)
    {
        var result = await _orderService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] OrderStatus status)
    {
        var order = await _uof.Orders.GetWithItemsAsync(id);

        if (order is null) return NotFound();

        order.Status = status;
        await _uof.SaveChangesAsync();

        return NoContent();
    }
}