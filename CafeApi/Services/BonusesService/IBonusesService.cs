using CafeApi.Models;

namespace CafeApi.Services.BonusesService;

public interface IBonusesService
{
    int Calculate(Order order);
}