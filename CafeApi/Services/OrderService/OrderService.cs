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

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto, int? promotionId = null)
    {
        var customer = await _uow.Customers.GetByIdAsync(dto.CustomerId);

        if (customer is null)
            throw new CustomerNotFound($"Customer with id {dto.CustomerId} not found");
        
        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = await MapOrderItemsAsync(dto.Items)
        };

        var total = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        order.FinalTotal = total;

        if (promotionId is not null)
        {
            await ApplyPromotionAsync(order, customer.Id, promotionId.Value, total);
        }
        
        await _uow.Orders.AddAsync(order);
        _uow.Customers.Update(customer);
        
        
        await _uow.SaveChangesAsync();
        var saved = await _uow.Orders.GetWithItemsAsync(order.Id);

        if (saved is null)
            throw new OrderNotFound($"Order with {order.Id} id not found");

        return _mapper.Map<OrderResponseDto>(saved);
    }
    
    private async Task<List<OrderItem>> MapOrderItemsAsync(IEnumerable<OrderItemDto> itemsDto)
    {
        var itemsDtoList = itemsDto.ToList();
        var ids = itemsDtoList.Select(x => x.MenuItemId).ToList();
        var menuItems = (await _uow.MenuItems.GetManyAsync(ids)).ToList();

        var orderItems = new List<OrderItem>();
        foreach (var itemDto in itemsDtoList)
        {
            var menuItem = menuItems.FirstOrDefault(m => m.Id == itemDto.MenuItemId)
                           ?? throw new MenuItemNotFound($"Menu item with id {itemDto.MenuItemId} not found");

            if (!menuItem.IsAvailable)
                throw new InvalidOperationException($"{menuItem.Name} is unavailable");

            orderItems.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = menuItem.Price
            });
        }

        return orderItems;
    }
    
    private async Task ApplyPromotionAsync(Order order, int customerId, int promotionId, decimal total)
    {
        var promotion = await _uow.CustomerPromotions.GetByCustomerAndPromotionAsync(customerId, promotionId)
                        ?? throw new CustomerNotFound($"Customer promotion with id {promotionId} not found");

        if (promotion.IsUsed)
            throw new InvalidOperationException("This promotion has already been used");

        order.FinalTotal = promotion.Promotion.DiscountType switch
        {
            DiscountType.Percentage => total * (1 - promotion.Promotion.DiscountValue / 100),
            DiscountType.FixedAmount => Math.Max(0, total - promotion.Promotion.DiscountValue),
            _ => total
        };

        promotion.IsUsed = true;
        promotion.UsedAt = DateTime.UtcNow;
        
        promotion.Order = order;
    }

    public async Task Update(int id, OrderStatus status)
    {
        var order = await _uow.Orders.GetByIdAsync(id);

        if (order is null)
            throw new OrderNotFound($"Order with {id} id not found");

        if (order.Status != OrderStatus.Completed && status == OrderStatus.Completed)
        {
            order.Customer.BonusPoints += _bonusesService.Calculate(order);
        }

        order.Status = status;
        await _uow.SaveChangesAsync();
    }
}