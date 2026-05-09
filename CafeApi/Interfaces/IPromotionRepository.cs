using CafeApi.Models;

namespace CafeApi.Interfaces;

public interface IPromotionRepository : IRepository<Promotion>
{
    Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
}