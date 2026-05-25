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
                "MenuItemName",
                opt => opt
                    .MapFrom(src => src.MenuItem.Name)
            );

        CreateMap<Order, OrderResponseDto>()
            .ForCtorParam("CustomerName",
                opt =>
                    opt.MapFrom(src => src.Customer.Name)
            )
            .ForCtorParam("Total",
                opt =>
                    opt.MapFrom(src =>
                        src.Items.Sum(i => i.UnitPrice * i.Quantity)
                    )
            )
            .ForCtorParam(
                "Items", opt =>
                    opt.MapFrom(src => src.Items)
            );

        CreateMap<CustomerPromotion, CustomerPromotionDto>()
            .ForCtorParam("Promotion",
                opt =>
                    opt.MapFrom(src => src.Promotion)
            );
        CreateMap<Promotion, PromotionDto>();
    }
}