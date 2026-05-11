using CafeApi.DTOs;

namespace CafeApi.Services.CustomerPromotionService;

public interface ICustomerPromotionService
{
    Task<CustomerPromotionDto> GetById(int id);
    Task<IEnumerable<CustomerPromotionDto>> GetAll();
    Task<CustomerPromotionDto> BuyPromotion(BuyPromotionDto dto);
    Task<IEnumerable<CustomerPromotionDto>> GetByCustomerIdAsync(int customerId);
    Task<CustomerPromotionDto> GetByCustomerAndPromotionAsync(int customerId, int promotionId);
    Task<CustomerPromotionDto> GetByOrderAsync(int orderId);
    Task Delete(int id);
}