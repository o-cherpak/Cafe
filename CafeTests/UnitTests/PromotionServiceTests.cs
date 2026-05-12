using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Models;
using CafeApi.Repositories;
using CafeApi.Services.PromotionService;
using FluentAssertions;

namespace CafeTests.UnitTests;

public class PromotionServiceTests
{
    private readonly CafeDbContext _db;
    private readonly PromotionService _service;

    public PromotionServiceTests()
    {
        _db = TestDbContextFactory.Create();
        var uow = new UnitOfWork(_db);
        _service = new PromotionService(uow);
    }

    private async Task Seed()
    {
        _db.Promotions.AddRange(
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
                Name = "20-Off",
                BonusCost = 200,
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = 20,
                IsActive = false
            }
        );

        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllTest()
    {
        await Seed();

        var result = await _service.GetAll();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        await Seed();

        var result = await _service.GetById(1);

        result.Name.Should().Be("10%");
        result.BonusCost.Should().Be(100);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetById_NotFoundTest()
    {
        await Assert.ThrowsAsync<PromotionNotFound>(() => _service.GetById(99999));
    }

    [Fact]
    public async Task GetActivePromotionsTest()
    {
        await Seed();

        var result = await _service.GetActivePromotions();

        result.Should().HaveCount(1);
        result.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task CreateTest()
    {
        var dto = new CreatePromotionDto(
            "Free Coffee",
            "One free coffee",
            500,
            DiscountType.FixedAmount,
            50
        );

        var result = await _service.Create(dto);

        result.Name.Should().Be("Free Coffee");
        result.BonusCost.Should().Be(500);
        result.IsActive.Should().BeTrue();
        _db.Promotions.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateTest()
    {
        await Seed();

        await _service.Update(
            1,
            new UpdatePromotionDto(
                "New Name", null, 15, null
            )
        );

        var result = await _service.GetById(1);

        result.Name.Should().Be("New Name");
        result.DiscountValue.Should().Be(15);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Update_DeactivateTest()
    {
        await Seed();

        await _service.Update(
            1,
            new UpdatePromotionDto(
                null, null, null, false
            )
        );

        var result = await _service.GetById(1);

        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Update_NotFoundTest()
    {
        await Assert.ThrowsAsync<PromotionNotFound>(() =>
            _service.Update(99999,
                new UpdatePromotionDto(null, null, null, null)
            )
        );
    }
}