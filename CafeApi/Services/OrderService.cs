using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Interfaces;
using CafeApi.Models;

namespace CafeApi.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;

    public OrderService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public OrderResponseDto ToDto(Order order)
    {
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

        return dto;
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        //Customer
        var customer = await _uow.Customers.GetByIdAsync(dto.CustomerId);

        if (customer is null)
            throw new KeyNotFoundException("Customer is not found");

        //Order
        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = []
        };

        //MenuItem
        foreach (var itemDto in dto.Items)
        {
            var menuItem = await _uow.MenuItems.GetByIdAsync(itemDto.MenuItemId);

            if (menuItem is null)
                throw new KeyNotFoundException($"Item {itemDto.MenuItemId} not found");

            if (!menuItem.IsAvailable)
                throw new InvalidOperationException($"{menuItem.Name} is unavailable");

            order.Items.Add(
                new OrderItem
                {
                    MenuItemId = menuItem.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = menuItem.Price
                }
            );
        }

        //Bonus
        var total = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        customer.BonusPoints += (int)(total * 10);

        //Save
        await _uow.Orders.AddAsync(order);
        _uow.Customers.Update(customer);
        await _uow.SaveChangesAsync();

        var saved = await _uow.Orders.GetWithItemsAsync(order.Id);

        if (saved is null)
            throw new KeyNotFoundException("Order not found");

        return ToDto(saved);
    }
}