using CafeApi.Models;

namespace CafeApi.Interfaces;

public interface ICustomerPromotionRepository : IRepository<CustomerPromotion>
{
    Task<IEnumerable<CustomerPromotion>> GetByCustomerId(int customerId);
    Task<CustomerPromotion?> GetByCustomerAndPromotion(int customerId, int promotionId);
    Task<CustomerPromotion?> GetByOrder(int orderId);
}