using CafeApi.Data;
using CafeApi.Interfaces;
using CafeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Repositories;

public class CustomerPromotionRepository : Repository<CustomerPromotion>, ICustomerPromotionRepository
{
    public CustomerPromotionRepository(CafeDbContext db) : base(db)
    {
    }

    public override async Task<CustomerPromotion?> GetByIdAsync(int id)
    {
        return await Db.CustomerPromotions
            .Include(cp => cp.Promotion)
            .Include(cp => cp.Customer)
            .FirstOrDefaultAsync(cp => cp.Id == id);
    }

    public override async Task<IEnumerable<CustomerPromotion>> GetAllAsync()
    {
        return await Db.CustomerPromotions
            .Include(cp => cp.Promotion)
            .Include(cp => cp.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<CustomerPromotion>> GetByCustomerIdAsync(int customerId)
    {
        return await Db.CustomerPromotions
            .Where(p => p.CustomerId == customerId)
            .Include(p => p.Promotion)
            .ToListAsync();
    }

    public async Task<CustomerPromotion?> GetByCustomerAndPromotionAsync(int customerId, int promotionId)
    {
        return await Db.CustomerPromotions
            .Include(cp => cp.Promotion)
            .FirstOrDefaultAsync(p =>
                p.CustomerId == customerId && p.PromotionId == promotionId);
    }

    public async Task<CustomerPromotion?> GetByOrderAsync(int orderId)
    {
        return await Db.CustomerPromotions.FirstOrDefaultAsync(p =>
            p.UsedInOrderId == orderId);
    }
}