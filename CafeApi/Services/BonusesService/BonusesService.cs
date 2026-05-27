using CafeApi.Models;

namespace CafeApi.Services.BonusesService;

public class BonusesService : IBonusesService
{
    private const decimal Multiplier = 1;

    public int Calculate(Order order)
    {
        var total = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        
        var bonusPoints = (int)(total * 10 * Multiplier);
        return bonusPoints;
    }
}