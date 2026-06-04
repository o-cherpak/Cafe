using AutoMapper;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Interfaces;
using CafeApi.Models;
using CafeApi.Services.BonusesService;

namespace CafeApi.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBonusesService _bonusesService;

    public OrderService(IUnitOfWork uow, IMapper mapper, IBonusesService bonusesService)
    {
        _uow = uow;
        _mapper = mapper;
        _bonusesService = bonusesService;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAll
    (
        int? customerId,
        OrderStatus? status
    )
    {
        var orders = customerId.HasValue
            ? await _uow.Orders.GetOrderByCustomerIdAsync(customerId.Value)
            : await _uow.Orders.GetAllAsync();

        if (status.HasValue)
        {
            orders = orders.Where(o => o.Status == status.Value);
        }

        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetById(int id)
    {
        var order = await _uow.Orders.GetWithItemsAsync(id);

        if (order is null)
            throw new OrderNotFound($"Order with {id} id not found");

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetOrderByCustomerId(int id)
    {
        var orders = await _uow.Orders.GetOrderByCustomerIdAsync(id);

        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task Update(int id, OrderStatus status)
    {
        var order = await _uow.Orders.GetByIdAsync(id);

        if (order is null)
            throw new OrderNotFound($"Order with {id} id not found");

        if (order.Status != OrderStatus.Completed && status == OrderStatus.Completed)
        {
            order.Customer.AddBonusPoints(_bonusesService.Calculate(order));
        }

        order.UpdateStatus(status);
        await _uow.SaveChangesAsync();
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto, int? promotionId = null)
    {
        var customer = await _uow.Customers.GetByIdAsync(dto.CustomerId);

        if (customer is null)
            throw new CustomerNotFound($"Customer with id {dto.CustomerId} not found");

        var order = Order.Create(
            dto.CustomerId,
            await MapOrderItemsAsync(dto.Items)
        );

        if (promotionId is not null)
        {
            var promotion = await _uow.CustomerPromotions.GetByCustomerAndPromotionAsync(customer.Id, promotionId.Value)
                            ?? throw new CustomerNotFound($"Customer promotion with id {promotionId} not found");

            order.ApplyPromotion(promotion);
        }

        await _uow.Orders.AddAsync(order);
        await _uow.SaveChangesAsync();

        var saved = await _uow.Orders.GetWithItemsAsync(order.Id);

        if (saved is null)
            throw new OrderNotFound($"Order with {order.Id} id not found");

        return _mapper.Map<OrderResponseDto>(saved);
    }

    private async Task<List<OrderItem>> MapOrderItemsAsync(IEnumerable<OrderItemDto> itemsDto)
    {
        var groupedDto = itemsDto
            .GroupBy(x => x.MenuItemId)
            .Select(g => 
                new { MenuItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        var ids = groupedDto.Select(x => x.MenuItemId).ToList();
        var menuItems = (await _uow.MenuItems.GetManyAsync(ids)).ToDictionary(x => x.Id);

        return groupedDto.Select(dto =>
        {
            if (!menuItems.TryGetValue(dto.MenuItemId, out var menuItem))
                throw new MenuItemNotFound($"Menu item with id {dto.MenuItemId} not found");

            return OrderItem.Create(menuItem, dto.Quantity);
        }).ToList();
    }
}