using AutoMapper;
using CafeApi.DTOs;
using CafeApi.Models;

namespace CafeApi.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerDto>();
        
        CreateMap<MenuItem, MenuItemDto>();
        CreateMap<Order, OrderResponseDto>();
        CreateMap<OrderItem, OrderItemDto>(); 
        
        CreateMap<CustomerPromotion, CustomerPromotionDto>();
        CreateMap<Promotion, PromotionDto>();
    }
}