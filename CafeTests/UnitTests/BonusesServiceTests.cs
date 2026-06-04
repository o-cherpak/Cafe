using CafeApi.Models;
using CafeApi.Services.BonusesService;
using FluentAssertions;

namespace CafeTests.UnitTests;

public class BonusesServiceTests
{
    private readonly BonusesService _bonusesService;

    public BonusesServiceTests()
    {
        _bonusesService = new BonusesService();
    }

    [Fact]
    public void Calculate_ReturnZero()
    {
        var order = Order.Create(0, new List<OrderItem>());

        var result = _bonusesService.Calculate(order);

        result.Should().Be(0);
    }

    [Theory]
    [InlineData(75, 1, 750)]
    [InlineData(50, 3, 1500)]
    [InlineData(10.5, 2, 210)]
    public void Calculate_SingleItem_ShouldReturnCorrectPoints(decimal price, int quantity, int expectedPoints)
    {
        var items = new List<OrderItem> { OrderItem.Create(0, quantity, price) };
        var order = Order.Create(0, items);

        var result = _bonusesService.Calculate(order);
        result.Should().Be(expectedPoints);
    }
}