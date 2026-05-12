using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Models;
using CafeApi.Repositories;
using CafeApi.Services.CustomerPromotionService;
using FluentAssertions;

namespace CafeTests.UnitTests;

public class CustomerPromotionServiceTests
{
    private readonly CafeDbContext _db;
    private readonly CustomerPromotionService _service;
    private readonly List<Customer> _customers = [];
    private readonly List<Promotion> _promotions = [];

    public CustomerPromotionServiceTests()
    {
        _db = TestDbContextFactory.Create();
        var uow = new UnitOfWork(_db);
        _service = new CustomerPromotionService(uow);
    }

    private async Task Seed()
    {
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
                Name = "10% Off",
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

        _db.Customers.AddRange(_customers);
        _db.Promotions.AddRange(_promotions);
        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllTest()
    {
        await Seed();

        await _service.BuyPromotion(new BuyPromotionDto(_customers[0].Id, _promotions[0].Id));

        var result = await _service.GetAll();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        await Seed();

        var bought = await _service.BuyPromotion(
            new BuyPromotionDto(_customers[0].Id, _promotions[0].Id)
        );

        var result = await _service.GetById(bought.Id);

        result.Id.Should().Be(bought.Id);
        result.CustomerId.Should().Be(_customers[0].Id);
        result.Promotion.Name.Should().Be("10% Off");
    }

    [Fact]
    public async Task GetById_NotFoundTest()
    {
        await Assert.ThrowsAsync<CustomerPromotionNotFound>(() => _service.GetById(99999));
    }

    [Fact]
    public async Task BuyPromotionTest()
    {
        await Seed();

        var result = await _service.BuyPromotion(
            new BuyPromotionDto(_customers[0].Id, _promotions[0].Id)
        );

        result.CustomerId.Should().Be(_customers[0].Id);
        result.Promotion.Name.Should().Be("10% Off");
        result.IsUsed.Should().BeFalse();

        var updatedCustomer = await _db.Customers.FindAsync(_customers[0].Id);
        updatedCustomer!.BonusPoints.Should().Be(400);
    }

    [Fact]
    public async Task BuyPromotion_CustomerNotFoundTest()
    {
        await Seed();

        await Assert.ThrowsAsync<CustomerNotFound>(
            () => _service.BuyPromotion(new BuyPromotionDto(99999, _promotions[0].Id))
        );
    }

    [Fact]
    public async Task BuyPromotion_PromotionNotFoundTest()
    {
        await Seed();

        await Assert.ThrowsAsync<PromotionNotFound>(
            () => _service.BuyPromotion(new BuyPromotionDto(_customers[0].Id, 99999))
        );
    }

    [Fact]
    public async Task BuyPromotion_NotActiveTest()
    {
        await Seed();

        await Assert.ThrowsAsync<PromotionNotActiveException>(
            () => _service.BuyPromotion(new BuyPromotionDto(_customers[0].Id, _promotions[1].Id))
        );
    }

    [Fact]
    public async Task BuyPromotion_AlreadyHaveTest()
    {
        await Seed();

        await _service.BuyPromotion(new BuyPromotionDto(_customers[0].Id, _promotions[0].Id));

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.BuyPromotion(new BuyPromotionDto(_customers[0].Id, _promotions[0].Id))
        );
    }

    [Fact]
    public async Task BuyPromotion_InsufficientBonusTest()
    {
        await Seed();
        
        await Assert.ThrowsAsync<InsufficientBonusException>(
            () => _service.BuyPromotion(new BuyPromotionDto(_customers[1].Id, _promotions[0].Id))
        );
    }

    [Fact]
    public async Task GetByCustomerIdTest()
    {
        await Seed();

        await _service.BuyPromotion(new BuyPromotionDto(_customers[0].Id, _promotions[0].Id));

        var result = await _service.GetByCustomerIdAsync(_customers[0].Id);

        result.Should().HaveCount(1);
        result.Should().AllSatisfy(p => p.CustomerId.Should().Be(_customers[0].Id));
    }

    [Fact]
    public async Task GetByCustomerAndPromotionTest()
    {
        await Seed();

        await _service.BuyPromotion(new BuyPromotionDto(_customers[0].Id, _promotions[0].Id));

        var result = await _service.GetByCustomerAndPromotionAsync(
            _customers[0].Id, _promotions[0].Id
        );

        result.CustomerId.Should().Be(_customers[0].Id);
        result.Promotion.Id.Should().Be(_promotions[0].Id);
    }

    [Fact]
    public async Task DeleteTest()
    {
        await Seed();

        var bought = await _service.BuyPromotion(
            new BuyPromotionDto(_customers[0].Id, _promotions[0].Id)
        );

        await _service.Delete(bought.Id);

        _db.CustomerPromotions.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_NotFoundTest()
    {
        await Assert.ThrowsAsync<CustomerPromotionNotFound>(() => _service.Delete(99999));
    }
}