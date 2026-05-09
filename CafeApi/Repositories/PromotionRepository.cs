using CafeApi.Data;
using CafeApi.Interfaces;
using CafeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Repositories;

public class PromotionRepository : Repository<Promotion>, IPromotionRepository
{
    public PromotionRepository(CafeDbContext db) : base(db)
    {
    }

    public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
    {
        return await Db.Promotions.Where(p => p.IsActive).ToListAsync();
    }
}