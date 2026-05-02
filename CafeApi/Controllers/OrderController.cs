using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Interfaces;
using CafeApi.Models;
using CafeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly OrderService _orderService;

    public OrderController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] OrderStatus? status
    )
    {
        IEnumerable<Order> orders;

        if (customerId.HasValue && status.HasValue)
        {
            orders = await _uof.Orders.GetOrderByCustomerIdAsync(customerId.Value);
            orders = orders.Where(order => order.Status == status.Value);
        }
        else if (customerId.HasValue && !status.HasValue)
        {
            orders = await _uof.Orders.GetOrderByCustomerIdAsync(customerId.Value);
        }
        else if (!customerId.HasValue && status.HasValue)
        {
            orders = await _uof.Orders.GetOrderByStatusAsync(status.Value);
        }
        else
        {
            orders = await _uof.Orders.GetAllAsync();
        }

        var result = orders.Select(order => new OrderResponseDto(
            order.Id,
            order.Customer.Name,
            order.Status,
            order.CreatedAt,
            Total: order.Items.Sum(i => i.UnitPrice * i.Quantity),
            Items: order.Items.Select(i =>
                new OrderItemResponseDto(
                    i.MenuItem.Name,
                    i.Quantity,
                    i.UnitPrice
                )
            ).ToList()
        ));

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var order = await _uof.Orders.GetByIdAsync(id);

        if (order is null) return NotFound();

        var dto = new OrderResponseDto(
            order.Id, order.Customer.Name,
            order.Status,
            order.CreatedAt,
            Total: order.Items.Sum(i => i.UnitPrice * i.Quantity),
            Items: order.Items.Select(i =>
                new OrderItemResponseDto(
                    i.MenuItem.Name,
                    i.Quantity,
                    i.UnitPrice
                )
            ).ToList()
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create(CreateOrderDto dto)
    {
        var result = await _orderService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut]
    public async Task<ActionResult> Update(int id, OrderStatus status)
    {
        var order = await _uof.Orders.GetByIdAsync(id);

        if (order is null) return NotFound();

        order.Status = status;
        await _uof.SaveChangesAsync();

        return NoContent();
    }
}