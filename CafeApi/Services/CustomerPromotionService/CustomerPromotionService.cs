using AutoMapper;
using CafeApi.DTOs;
using CafeApi.Exceptions;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Interfaces;
using CafeApi.Models;

namespace CafeApi.Services.CustomerPromotionService;

public class CustomerPromotionService : ICustomerPromotionService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CustomerPromotionService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CustomerPromotionDto> GetById(int id)
    {
        var promotion = await _uow.CustomerPromotions.GetByIdAsync(id);

        if (promotion is null)
            throw new CustomerPromotionNotFound($"Customer Promotion with {id} id not found");

        return _mapper.Map<CustomerPromotionDto>(promotion);
    }

    public async Task<IEnumerable<CustomerPromotionDto>> GetAll()
    {
        var promotions = await _uow.CustomerPromotions.GetAllAsync();

        return _mapper.Map<IEnumerable<CustomerPromotionDto>>(promotions);
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

        customer.SubtractBonusPoints(promotion.BonusCost);

        var newPromotion = new CustomerPromotion
        {
            CustomerId = dto.CustomerId,
            PromotionId = dto.PromotionId,
            PurchasedAt = DateTime.UtcNow
        };

        _uow.Customers.Update(customer);

        await _uow.CustomerPromotions.AddAsync(newPromotion);
        await _uow.SaveChangesAsync();

        var saved = await _uow.CustomerPromotions.GetByIdAsync(newPromotion.Id);

        return _mapper.Map<CustomerPromotionDto>(saved);
    }

    public async Task<IEnumerable<CustomerPromotionDto>> GetByCustomerIdAsync(int customerId)
    {
        var customer = await _uow.Customers.GetByIdAsync(customerId);
        if (customer is null)
        {
            throw new CustomerNotFound($"Customer with {customerId} id not found");
        }

        var promotions = await _uow.CustomerPromotions.GetByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<CustomerPromotionDto>>(promotions);
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

        return _mapper.Map<CustomerPromotionDto>(result);
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

        return _mapper.Map<CustomerPromotionDto>(result);
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