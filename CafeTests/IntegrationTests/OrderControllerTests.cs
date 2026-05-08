using System.Net;
using System.Net.Http.Json;
using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTests.IntegrationTests;

public class OrderControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private List<Customer> _customerList = new List<Customer>()!;
    private List<MenuItem> _menuList = new List<MenuItem>();

    public OrderControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
        db.Database.EnsureDeleted();

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

        db.Customers.AddRange(_customerList);
        db.MenuItems.AddRange(_menuList);

        db.SaveChanges();
    }

    [Fact]
    public async Task GetAllTest()
    {
        var response = await _client.GetAsync("/api/order");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetByIdTest()
    {
        var dto = new CreateOrderDto(
            _customerList[0].Id,
            [new OrderItemDto(_menuList[0].Id, 2)]
        );

        var createResponse = await _client.PostAsJsonAsync("/api/order", dto);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

        var response = await _client.GetAsync($"/api/order/{created!.Id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        result!.Id.Should().Be(created.Id);
        result.CustomerName.Should().Be(_customerList[0].Name);
    }

    [Fact]
    public async Task GetOrder_ByCustomerIdTest()
    {
        var customer = _customerList[0];

        var dto1 = new CreateOrderDto(
            customer.Id,
            [new OrderItemDto(_menuList[1].Id, 4)]
        );

        var dto2 = new CreateOrderDto(
            customer.Id,
            [new OrderItemDto(_menuList[1].Id, 4)]
        );

        var createResponse1 = await _client.PostAsJsonAsync("/api/order", dto1);
        var createResponse2 = await _client.PostAsJsonAsync("/api/order", dto2);
        createResponse1.EnsureSuccessStatusCode();
        createResponse2.EnsureSuccessStatusCode();

        var response = await _client.GetAsync("/api/order/by-customer?customerId=1");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();

        result.Should().HaveCount(2);

        result.Should().AllSatisfy(order => { order.CustomerName.Should().Be(customer.Name); });
    }

    [Fact]
    public async Task GetById_NotFoundTest()
    {
        var response = await _client.GetAsync("/api/order/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTest()
    {
        var dto = new CreateOrderDto(
            _customerList[0].Id,
            [new OrderItemDto(_menuList[0].Id, 4)]
        );

        var response = await _client.PostAsJsonAsync("/api/order", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();

        result!.Items.First().Quantity.Should().Be(4);
        result.CustomerName.Should().Be(_customerList[0].Name);
    }

    [Fact]
    public async Task UpdateTest()
    {
        var createDto = new CreateOrderDto(
            _customerList[1].Id,
            [
                new OrderItemDto(_menuList[0].Id, 2),
                new OrderItemDto(_menuList[1].Id, 4)
            ]
        );

        var createResponse = await _client.PostAsJsonAsync("/api/order", createDto);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

        var putResponse = await _client.PutAsJsonAsync($"/api/order/{created!.Id}", OrderStatus.Completed);
        putResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/order/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var result = await getResponse.Content.ReadFromJsonAsync<OrderResponseDto>();


        result!.Items.First().Quantity.Should().Be(2);
        result.CustomerName.Should().Be(_customerList[1].Name);
        result.Status.Should().Be(OrderStatus.Completed);
    }
}