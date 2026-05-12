using System.Net;
using System.Net.Http.Json;
using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTests.IntegrationTests;

public class CustomerPromotionControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly List<Customer> _customers = [];
    private readonly List<Promotion> _promotions = [];

    public CustomerPromotionControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        _customers.AddRange(
            new Customer
            {
                Name = "Alex",
                Email = "alex@gmail.com",
                BonusPoints = 500,
                RegisteredAt = DateTime.UtcNow
            },
            new Customer
            {
                Name = "Gabriel",
                Email = "gabriel@gmail.com",
                BonusPoints = 50,
                RegisteredAt = DateTime.UtcNow
            }
        );

        _promotions.AddRange(
            new Promotion
            {
                Name = "10%",
                BonusCost = 100,
                DiscountType = DiscountType.Percentage,
                DiscountValue = 10,
                IsActive = true
            },
            new Promotion
            {
                Name = "Inactive Promo",
                BonusCost = 100,
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = 20,
                IsActive = false
            }
        );

        db.Customers.AddRange(_customers);
        db.Promotions.AddRange(_promotions);
        db.SaveChanges();
    }

    [Fact]
    public async Task GetAllTest()
    {
        var response = await _client.GetAsync("/api/customerpromotion");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task BuyPromotion_Test()
    {
        var dto = new BuyPromotionDto(_customers[0].Id, _promotions[0].Id);

        var response = await _client.PostAsJsonAsync("/api/customerpromotion", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CustomerPromotionDto>();
        result!.CustomerId.Should().Be(_customers[0].Id);
        result.Promotion.Name.Should().Be("10%");
        result.IsUsed.Should().BeFalse();
    }

    [Fact]
    public async Task BuyPromotion_CustomerNotFoundTest()
    {
        var dto = new BuyPromotionDto(99999, _promotions[0].Id);

        var response = await _client.PostAsJsonAsync("/api/customerpromotion", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BuyPromotion_NotActiveTest()
    {
        var dto = new BuyPromotionDto(_customers[0].Id, _promotions[1].Id);

        var response = await _client.PostAsJsonAsync("/api/customerpromotion", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BuyPromotion_InsufficientBonusTest()
    {
        var dto = new BuyPromotionDto(_customers[1].Id, _promotions[0].Id);

        var response = await _client.PostAsJsonAsync("/api/customerpromotion", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BuyPromotion_AlreadyHaveTest()
    {
        var dto = new BuyPromotionDto(_customers[0].Id, _promotions[0].Id);

        await _client.PostAsJsonAsync("/api/customerpromotion", dto);
        var response = await _client.PostAsJsonAsync("/api/customerpromotion", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        var buyDto = new BuyPromotionDto(_customers[0].Id, _promotions[0].Id);
        var buyResponse = await _client.PostAsJsonAsync("/api/customerpromotion", buyDto);
        var bought = await buyResponse.Content.ReadFromJsonAsync<CustomerPromotionDto>();

        var response = await _client.GetAsync($"/api/customerpromotion/{bought!.Id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CustomerPromotionDto>();
        result!.Id.Should().Be(bought.Id);
        result.Promotion.Name.Should().Be("10%");
    }

    [Fact]
    public async Task GetById_NotFoundTest()
    {
        var response = await _client.GetAsync("/api/customerpromotion/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCustomerIdTest()
    {
        var dto = new BuyPromotionDto(_customers[0].Id, _promotions[0].Id);
        await _client.PostAsJsonAsync("/api/customerpromotion", dto);

        var response = await _client.GetAsync(
            $"/api/customerpromotion/by-customer?customerId={_customers[0].Id}"
        );
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<CustomerPromotionDto>>();
        result.Should().HaveCount(1);
        result!.First().CustomerId.Should().Be(_customers[0].Id);
    }

    [Fact]
    public async Task DeleteTest()
    {
        var buyDto = new BuyPromotionDto(_customers[0].Id, _promotions[0].Id);
        var buyResponse = await _client.PostAsJsonAsync("/api/customerpromotion", buyDto);
        var bought = await buyResponse.Content.ReadFromJsonAsync<CustomerPromotionDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/customerpromotion/{bought!.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/customerpromotion/{bought.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NotFoundTest()
    {
        var response = await _client.DeleteAsync("/api/customerpromotion/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}