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

        CreateMap<OrderItem, OrderItemResponseDto>()
            .ForCtorParam(
                "menuItemName",
                opt => opt
                    .MapFrom(src => src.MenuItem.Name)
            );

        CreateMap<Order, OrderResponseDto>()
            .ForCtorParam("customerName",
                opt =>
                    opt.MapFrom(src => src.Customer.Name)
            )
            .ForCtorParam("total",
                opt =>
                    opt.MapFrom(src =>
                        src.Items.Sum(i => i.UnitPrice * i.Quantity)
                    )
            )
            .ForCtorParam(
                "items", opt =>
                    opt.MapFrom(src => src.Items)
            );

        CreateMap<CustomerPromotion, CustomerPromotionDto>();
        CreateMap<Promotion, PromotionDto>();
    }
}