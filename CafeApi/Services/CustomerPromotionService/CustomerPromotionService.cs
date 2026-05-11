using CafeApi.DTOs;
using CafeApi.Exceptions;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Interfaces;
using CafeApi.Models;

namespace CafeApi.Services.CustomerPromotionService;

public class CustomerPromotionService : ICustomerPromotionService
{
    private readonly IUnitOfWork _uow;

    public CustomerPromotionService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private CustomerPromotionDto ToDto(CustomerPromotion promotion)
    {
        var promotionDto = new PromotionDto(
            promotion.Id,
            promotion.Promotion.Name,
            promotion.Promotion.Description,
            promotion.Promotion.BonusCost,
            promotion.Promotion.DiscountType,
            promotion.Promotion.DiscountValue,
            promotion.Promotion.IsActive
        );

        return new CustomerPromotionDto(
            promotion.Id,
            promotion.CustomerId,
            promotionDto,
            promotion.IsUsed,
            promotion.PurchasedAt,
            promotion.UsedAt,
            promotion.UsedInOrderId
        );
    }

    public async Task<CustomerPromotionDto> GetById(int id)
    {
        var promotion = await _uow.CustomerPromotions.GetByIdAsync(id);

        if (promotion is null)
            throw new CustomerPromotionNotFound($"Customer Promotion with {id} id not found");

        return ToDto(promotion);
    }

    public async Task<IEnumerable<CustomerPromotionDto>> GetAll()
    {
        var result = await _uow.CustomerPromotions.GetAllAsync();

        return result.Select(ToDto);
    }

    public async Task<CustomerPromotionDto> BuyPromotion(BuyPromotionDto dto)
    {
        var customer = await _uow.Customers.GetByIdAsync(dto.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFound($"Customer with {dto.CustomerId} id not found");
        }

        var promotion = await _uow.Promotions.GetByIdAsync(dto.PromotionId);
        if (promotion is null)
        {
            throw new PromotionNotFound($"Promotion with {dto.PromotionId} id not found");
        }

        if (!promotion.IsActive)
        {
            throw new PromotionNotActiveException("This promotion is not active.");
        }

        var existing = await _uow.CustomerPromotions
            .GetByCustomerAndPromotionAsync(dto.CustomerId, dto.PromotionId);

        if (existing is not null)
            throw new ConflictException("Customer already owns this promotion");

        if (customer.BonusPoints < promotion.BonusCost)
            throw new InsufficientBonusException(
                $"Not enough bonus points. Required: {promotion.BonusCost}, available: {customer.BonusPoints}"
            );

        var newPromotion = new CustomerPromotion
        {
            CustomerId = dto.CustomerId,
            PromotionId = dto.PromotionId,
            PurchasedAt = DateTime.UtcNow
        };

        customer.BonusPoints -= promotion.BonusCost;
        _uow.Customers.Update(customer);

        await _uow.CustomerPromotions.AddAsync(newPromotion);
        await _uow.SaveChangesAsync();

        var saved = await _uow.CustomerPromotions.GetByIdAsync(newPromotion.Id);

        return ToDto(saved!);
    }

    public async Task<IEnumerable<CustomerPromotionDto>> GetByCustomerIdAsync(int customerId)
    {
        var customer = await _uow.Customers.GetByIdAsync(customerId);
        if (customer is null)
        {
            throw new CustomerNotFound($"Customer with {customerId} id not found");
        }

        var result = await _uow.CustomerPromotions.GetByCustomerIdAsync(customerId);

        return result.Select(ToDto);
    }

    public async Task<CustomerPromotionDto> GetByCustomerAndPromotionAsync(int customerId, int promotionId)
    {
        var customer = await _uow.Customers.GetByIdAsync(customerId);
        if (customer is null)
        {
            throw new CustomerNotFound($"Customer with {customerId} customerId id not found");
        }

        var promotion = await _uow.Promotions.GetByIdAsync(promotionId);
        if (promotion is null)
        {
            throw new PromotionNotFound($"Promotion with {promotionId} promotionId id not found");
        }

        var result = await _uow.CustomerPromotions
            .GetByCustomerAndPromotionAsync(customerId, promotionId);

        if (result is null)
            throw new CustomerPromotionNotFound(
                $"Customer with {promotionId} promotionId and with {customerId} customerId"
            );

        return ToDto(result);
    }

    public async Task<CustomerPromotionDto> GetByOrderAsync(int orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order is null)
        {
            throw new OrderNotFound($"Order with {orderId} id not found");
        }

        var result = await _uow.CustomerPromotions.GetByOrderAsync(orderId);

        if (result is null)
            throw new CustomerPromotionNotFound($"Customer Promotion with {orderId} order id not found");

        return ToDto(result);
    }

    public async Task Delete(int id)
    {
        var promotion = await _uow.CustomerPromotions.GetByIdAsync(id);

        if (promotion is null)
            throw new CustomerPromotionNotFound($"Customer Promotion with {id} id not found");

        _uow.CustomerPromotions.Delete(promotion);

        await _uow.SaveChangesAsync();
    }
}