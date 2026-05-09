using CafeApi.DTOs;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Interfaces;
using CafeApi.Models;

namespace CafeApi.Services.PromotionService;

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _uow;

    public PromotionService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private PromotionDto ToDto(Promotion promotion)
    {
        return new PromotionDto(
            promotion.Id,
            promotion.Name,
            promotion.Description,
            promotion.BonusCost,
            promotion.DiscountType,
            promotion.DiscountValue,
            promotion.IsActive
        );
    }

    public async Task<PromotionDto> GetById(int id)
    {
        var promotion = await _uow.Promotions.GetByIdAsync(id);

        if (promotion is null) throw new PromotionNotFound($"Promotion with {id} id not found");

        return ToDto(promotion);
    }

    public async Task<IEnumerable<PromotionDto>> GetActivePromotions()
    {
        var promotions = await _uow.Promotions.GetActivePromotionsAsync();

        return promotions.Select(ToDto);
    }

    public async Task<IEnumerable<PromotionDto>> GetAll()
    {
        var promotions = await _uow.Promotions.GetAllAsync();

        return promotions.Select(ToDto);
    }

    public async Task<PromotionDto> Create(CreatePromotionDto dto)
    {
        var promotion = new Promotion
        {
            Name = dto.Name,
            Description = dto.Description,
            DiscountType = dto.DiscountType,
            DiscountValue = dto.DiscountValue,
            BonusCost = dto.BonusCost,
            IsActive = true
        };

        await _uow.Promotions.AddAsync(promotion);
        await _uow.SaveChangesAsync();

        return ToDto(promotion);
    }

    public async Task Update(int id, UpdatePromotionDto dto)
    {
        var promotion = await _uow.Promotions.GetByIdAsync(id);

        if (promotion is null) throw new PromotionNotFound($"Promotion with {id} id not found");

        if (dto.IsActive is not null) promotion.IsActive = dto.IsActive.Value;
        if (dto.Description is not null) promotion.Description = dto.Description;
        if (dto.DiscountValue is not null) promotion.DiscountValue = dto.DiscountValue.Value;
        if (dto.Name is not null) promotion.Name = dto.Name;

        await _uow.SaveChangesAsync();
    }
}