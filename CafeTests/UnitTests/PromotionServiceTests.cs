using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Models;
using CafeApi.Repositories;
using CafeApi.Services.PromotionService;
using CafeApi.Validators.PromotionValidators;
using CafeTests.Data;
using CafeTests.Helpers;
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
        var mapper = TestMapperFactory.Create();
        _service = new PromotionService(uow, mapper);
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
    public async Task Validator_CreateNoErrorsTest()
    {
        var dto = new CreatePromotionDto(
            "Free Coffee",
            "Valid description",
            10,
            DiscountType.Percentage,
            10.0m
        );
        var validator = new CreatePromotionValidator();
        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_CreateInvalidTest()
    {
        var dto = new CreatePromotionDto(
            "",
            "gfgfger",
            -1,
            (DiscountType)99,
            -5.0m
        );

        var validator = new CreatePromotionValidator();
        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "BonusCost");
        result.Errors.Should().Contain(e => e.PropertyName == "DiscountType");
        result.Errors.Should().Contain(e => e.PropertyName == "DiscountValue");
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
    public async Task Validator_UpdateNoErrorsTest()
    {
        var dto = new UpdatePromotionDto(
            "Summer Sale",
            "Valid description",
            15.0m,
            true
        );
        var validator = new UpdatePromotionValidator();
        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_UpdateInvalidTest()
    {
        var dto = new UpdatePromotionDto(
            "",
            "description bom bom bom",
            -5.0m,
            null
        );

        var validator = new UpdatePromotionValidator();
        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "DiscountValue");
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