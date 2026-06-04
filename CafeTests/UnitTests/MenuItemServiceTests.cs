using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Models;
using CafeApi.Repositories;
using CafeApi.Services.MenuItemService;
using CafeApi.Validators;
using CafeTests.Data;
using CafeTests.Helpers;
using FluentAssertions;

namespace CafeTests.UnitTests;

public class MenuItemServiceTests
{
    private readonly CafeDbContext _db;
    private readonly MenuItemService _service;
    private readonly List<MenuItem> _menuItems = [];

    public MenuItemServiceTests()
    {
        _db = TestDbContextFactory.Create();

        var uow = new UnitOfWork(_db);
        var mapper = TestMapperFactory.Create();
        _service = new MenuItemService(uow, mapper);
    }

    private async Task Seed()
    {
        _menuItems.AddRange(
            MenuItem.Create(
                "Latte",
                ItemCategory.Beverages,
                75,
                "Milk coffee"
            ),
            MenuItem.Create(
                "Espresso",
                ItemCategory.Beverages,
                45,
                "Strong coffee"
            ),
            MenuItem.Create(
                "Cake",
                ItemCategory.Food,
                50,
                "Sweet dessert"
            ),
            MenuItem.Create(
                "Sandwich",
                ItemCategory.Food,
                65,
                "Tuna sandwich"
            )
        );

        _db.MenuItems.AddRange(_menuItems);
        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllTest()
    {
        await Seed();

        var result = await _service.GetAll(null);

        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetAllWithFilterTest()
    {
        await Seed();

        var result1 = await _service.GetAll(ItemCategory.Food);
        var result2 = await _service.GetAll(ItemCategory.Beverages);

        result1.Should().HaveCount(2);
        result2.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        await Seed();

        var result1 = await _service.GetById(_menuItems[0].Id);
        var result2 = await _service.GetById(_menuItems[2].Id);

        result1.Name.Should().Be("Latte");
        result1.Category.Should().Be(ItemCategory.Beverages);
        result1.Price.Should().Be(75);

        result2.Name.Should().Be("Cake");
        result2.Category.Should().Be(ItemCategory.Food);
        result2.Price.Should().Be(50);
    }

    [Fact]
    public async Task GetByIdWhenNotFoundTest()
    {
        await Assert.ThrowsAsync<MenuItemNotFound>(() => _service.GetById(99999));
    }

    [Fact]
    public async Task CreateTest()
    {
        var dto1 = new CreateMenuItemDto(
            "Espresso",
            ItemCategory.Beverages,
            45,
            null
        );
        var dto2 = new CreateMenuItemDto(
            "Latte",
            ItemCategory.Beverages,
            70,
            "Very tasty"
        );

        var result1 = await _service.Create(dto1);
        var result2 = await _service.Create(dto2);

        result1.Name.Should().Be("Espresso");
        result1.Price.Should().Be(45);

        result2.Name.Should().Be("Latte");
        result2.Price.Should().Be(70);
        _db.MenuItems.Should().HaveCount(2);
    }

    [Fact]
    public async Task Validator_InvalidPriceTest()
    {
        var validator = new CreateMenuItemValidator();
        var dto = new CreateMenuItemDto(
            "Espresso",
            ItemCategory.Beverages,
            0,
            null
        );

        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task Validator_EmptyNameTest()
    {
        var validator = new CreateMenuItemValidator();
        var dto = new CreateMenuItemDto(
            "",
            ItemCategory.Beverages,
            45,
            null
        );

        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task UpdateTest()
    {
        await Seed();

        await _service.Update(
            _menuItems[0].Id,
            new UpdateMenuItemDto(
                "New Latte",
                80,
                true,
                null,
                null
            )
        );
        await _service.Update(
            _menuItems[2].Id,
            new UpdateMenuItemDto(
                "New Cake",
                55,
                false,
                null,
                null
            )
        );

        var result1 = await _service.GetById(_menuItems[0].Id);
        var result2 = await _service.GetById(_menuItems[2].Id);

        result1.Name.Should().Be("New Latte");
        result1.Price.Should().Be(80);
        result1.IsAvailable.Should().BeTrue();

        result2.Name.Should().Be("New Cake");
        result2.Price.Should().Be(55);
        result2.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Validator_UpdateInvalidPriceTest()
    {
        var validator = new UpdateMenuItemValidator();
        var dto = new UpdateMenuItemDto(
            null,
            -10,
            null,
            null,
            null
        );

        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task Validator_UpdateNoErrorsTest()
    {
        var validator = new UpdateMenuItemValidator();
        var dto = new UpdateMenuItemDto(
            "New Latte",
            80,
            true,
            null,
            null
        );

        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTest()
    {
        await Seed();

        await _service.Delete(_menuItems[0].Id);

        _db.MenuItems.Should().HaveCount(3);
        _db.MenuItems.Should().NotContain(m => m.Name == "Latte");
    }
}