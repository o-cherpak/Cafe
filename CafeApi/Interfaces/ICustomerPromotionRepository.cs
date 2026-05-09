using CafeApi.Models;

namespace CafeApi.Interfaces;

public interface ICustomerPromotionRepository : IRepository<CustomerPromotion>
{
    Task<IEnumerable<CustomerPromotion>> GetByCustomerIdAsync(int customerId);
    Task<CustomerPromotion?> GetByCustomerAndPromotionAsync(int customerId, int promotionId);
    Task<CustomerPromotion?> GetByOrderAsync(int orderId);
}