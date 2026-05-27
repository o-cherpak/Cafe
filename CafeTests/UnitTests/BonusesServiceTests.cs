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
        var order = new Order { Items = new List<OrderItem>() };
        
        var result = _bonusesService.Calculate(order);
        
        result.Should().Be(0);
    }
    
    [Theory]
    [InlineData(75, 1, 750)]
    [InlineData(50, 3, 1500)]
    [InlineData(10.5, 2, 210)]
    public void Calculate_SingleItem_ShouldReturnCorrectPoints(decimal price, int quantity, int expectedPoints)
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { UnitPrice = price, Quantity = quantity }
            }
        };
        
        var result = _bonusesService.Calculate(order);
        result.Should().Be(expectedPoints);
    }
}