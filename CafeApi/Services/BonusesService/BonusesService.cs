using CafeApi.Models;

namespace CafeApi.Services.BonusesService;

public class BonusesService : IBonusesService
{
    private const decimal Multiplier = 1;

    public int Calculate(Order order)
    {
        var bonusPoints = (int)(order.FinalTotal * 10 * Multiplier);
        return bonusPoints;
    }
}