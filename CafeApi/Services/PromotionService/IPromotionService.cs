using CafeApi.DTOs;

namespace CafeApi.Services.PromotionService;

public interface IPromotionService
{
    Task<PromotionDto> GetById(int id);
    Task<IEnumerable<PromotionDto>> GetActivePromotions();
    Task<IEnumerable<PromotionDto>> GetAll();
    Task<PromotionDto> Create(CreatePromotionDto dto);
    Task Update(int id, UpdatePromotionDto dto);
}