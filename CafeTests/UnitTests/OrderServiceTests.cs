using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Models;
using CafeApi.Repositories;
using CafeApi.Services.OrderService;
using FluentAssertions;

namespace CafeTests.UnitTests;

public class OrderServiceTests
{
    private readonly CafeDbContext _db;
    private readonly OrderService _service;
    private List<Customer> _customerList = new List<Customer>()!;
    private List<MenuItem> _menuList = new List<MenuItem>();

    public OrderServiceTests()
    {
        _db = TestDbContextFactory.Create();

        var uow = new UnitOfWork(_db);
        _service = new OrderService(uow);
    }

    private async Task Seed()
    {
        _customerList.AddRange(
            new Customer
            {
                Name = "User",
                Email = "test@test.com",
                BonusPoints = 0,
                RegisteredAt = DateTime.UtcNow
            },
            new Customer
            {
                Name = "User2",
                Email = "test@test2.com",
                BonusPoints = 0,
                RegisteredAt = DateTime.UtcNow
            }
        );

        _menuList.AddRange(
            new MenuItem
            {
                Name = "Latte", Category = ItemCategory.Beverages, Price = 75
            },
            new MenuItem { Name = "Cake", Category = ItemCategory.Food, Price = 50 }
        );

        _db.Customers.AddRange(_customerList);
        _db.MenuItems.AddRange(_menuList);

        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_CustomerIdTest()
    {
        await Seed();

        await _service.CreateAsync(new CreateOrderDto(
            _customerList[0].Id,
            [new OrderItemDto(_menuList[0].Id, 1)]
        ));

        await _service.CreateAsync(new CreateOrderDto(
            _customerList[0].Id,
            [new OrderItemDto(_menuList[1].Id, 2)]
        ));


        var result1 = await _service.GetAll(_customerList[0].Id, null);
        var result2 = await _service.GetAll(_customerList[1].Id, null);

        result1.Should().HaveCount(2);
        result2.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetAll_StatusTest()
    {
        await Seed();

        var order = await _service.CreateAsync(new CreateOrderDto(
            _customerList[0].Id,
            [
                new OrderItemDto(_menuList[0].Id, 1),
                new OrderItemDto(_menuList[1].Id, 3)
            ]
        ));

        await _service.CreateAsync(new CreateOrderDto(
            _customerList[0].Id,
            [new OrderItemDto(_menuList[0].Id, 1)]
        ));

        await _service.Update(order.Id, OrderStatus.Cancelled);

        var result = await _service.GetAll(_customerList[0].Id, OrderStatus.Pending);
        var ordersList = result.ToList();

        ordersList.Should().NotBeEmpty();
        ordersList.Select(o => o.Status).Should().NotContain(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task GetIdTest()
    {
        await Seed();

        var order = await _service.CreateAsync(new CreateOrderDto(
            _customerList[0].Id,
            [
                new OrderItemDto(_menuList[0].Id, 1),
                new OrderItemDto(_menuList[1].Id, 3)
            ]
        ));

        var result = await _service.GetById(order.Id);

        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task Create_AddBonusesTest()
    {
        await Seed();

        var dto = new CreateOrderDto(
            _customerList[0].Id,
            [new OrderItemDto(_menuList[0].Id, 1)]
        );

        var createOrder = await _service.CreateAsync(dto);

        _db.Orders.Should().HaveCount(1);
        var foundedOrder = _db.Orders.First();

        foundedOrder.Id.Should().Be(createOrder.Id);
        var updatedCustomer = await _db.Customers.FindAsync(_customerList[0].Id);
        updatedCustomer!.BonusPoints.Should().Be(750);
    }

    [Fact]
    public async Task UpdateStatusTest()
    {
        await Seed();

        var dto = new CreateOrderDto(
            _customerList[0].Id,
            [new OrderItemDto(_menuList[0].Id, 3)]
        );

        var order = await _service.CreateAsync(dto);
        await _service.Update(order.Id, OrderStatus.Cancelled);

        var updatedOrder = await _service.GetById(order.Id);

        updatedOrder.Id.Should().Be(order.Id);
        updatedOrder.Items.First().Quantity.Should().Be(3);
        updatedOrder.Status.Should().Be(OrderStatus.Cancelled);
    }
    
}