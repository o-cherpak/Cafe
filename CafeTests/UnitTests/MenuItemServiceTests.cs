using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Models;
using CafeApi.Repositories;
using CafeApi.Services.MenuItemService;
using FluentAssertions;

namespace CafeTests.UnitTests;

public class MenuItemServiceTests
{
    private readonly CafeDbContext _db;
    private readonly MenuItemService _service;

    public MenuItemServiceTests()
    {
        _db = TestDbContextFactory.Create();

        var uow = new UnitOfWork(_db);
        _service = new MenuItemService(uow);
    }
    
    [Fact]
    public async Task GetAllTest()
    {
        _db.MenuItems.AddRange(
            new MenuItem { Name = "Latte", Category = ItemCategory.Beverages, Price = 75 },
            new MenuItem { Name = "Cake", Category = ItemCategory.Food, Price = 50 }
        );

        await _db.SaveChangesAsync();

        var result = await _service.GetAll(null);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllWithFilterTest()
    {
        _db.MenuItems.AddRange(
            new MenuItem { Name = "Latte", Category = ItemCategory.Beverages, Price = 75 },
            new MenuItem { Name = "Cake1", Category = ItemCategory.Food, Price = 50 },
            new MenuItem { Name = "Cake2", Category = ItemCategory.Food, Price = 51 },
            new MenuItem { Name = "Cake3", Category = ItemCategory.Food, Price = 52 }
        );

        await _db.SaveChangesAsync();

        var result1 = await _service.GetAll(ItemCategory.Food);
        var result2 = await _service.GetAll(ItemCategory.Beverages);

        result1.Should().HaveCount(3);
        result2.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        _db.MenuItems.AddRange(
            new MenuItem { Name = "Latte", Category = ItemCategory.Beverages, Price = 75 },
            new MenuItem { Name = "Cake", Category = ItemCategory.Food, Price = 50 }
        );

        await _db.SaveChangesAsync();

        var result1 = await _service.GetById(1);
        var result2 = await _service.GetById(2);

        result1.Should().BeEquivalentTo(new
        {
            Name = "Latte",
            Category = ItemCategory.Beverages,
            Price = 75
        });

        result2.Should().BeEquivalentTo(new
        {
            Name = "Cake",
            Category = ItemCategory.Food,
            Price = 50
        });
    }

    [Fact]
    public async Task GetByIdWhenNotFoundTest()
    {
        await Assert.ThrowsAsync<MenuItemNotFound>(() => _service.GetById(99999));
    }

    [Fact]
    public async Task CreateTest()
    {
        var dto1 = new CreateMenuItemDto("Espresso", ItemCategory.Beverages, 45, null);
        var dto2 = new CreateMenuItemDto("Latte", ItemCategory.Beverages, 70, "Very tasty");

        var result1 = await _service.Create(dto1);
        var result2 = await _service.Create(dto2);

        result1.Name.Should().Be("Espresso");
        result1.Price.Should().Be(45);

        result2.Name.Should().Be("Latte");
        result2.Price.Should().Be(70);
        _db.MenuItems.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateTest()
    {
        _db.MenuItems.AddRange(
            new MenuItem { Name = "New Latte", Category = ItemCategory.Beverages, Price = 70 },
            new MenuItem { Name = "Cake", Category = ItemCategory.Food, Price = 45 }
        );

        await _db.SaveChangesAsync();

        await _service.Update(1, new UpdateMenuItemDto("Latte", 70, true));
        await _service.Update(2, new UpdateMenuItemDto("Cake", 50, false));

        var result1 = await _service.GetById(1);
        var result2 = await _service.GetById(2);

        _db.MenuItems.Should().HaveCount(2);

        result1.Should().BeEquivalentTo(new
        {
            Name = "Latte",
            Category = ItemCategory.Beverages,
            Price = 70,
            IsAvailable = true
        });

        result2.Should().BeEquivalentTo(new
        {
            Name = "Cake",
            Category = ItemCategory.Food,
            Price = 50,
            IsAvailable = false
        });
    }

    [Fact]
    public async Task DeleteTest()
    {
        _db.MenuItems.AddRange(
            new MenuItem { Name = "Latte", Category = ItemCategory.Beverages, Price = 75 },
            new MenuItem { Name = "Cake1", Category = ItemCategory.Food, Price = 50 },
            new MenuItem { Name = "Cake2", Category = ItemCategory.Food, Price = 51 },
            new MenuItem { Name = "Cake3", Category = ItemCategory.Food, Price = 52 }
        );

        await _db.SaveChangesAsync();

        await _service.Delete(1);

        _db.MenuItems.Should().HaveCount(3);

        _db.MenuItems.Should().NotContain(m => m.Name == "Latte");
    }
}