using AutoMapper;
using CartService.Entities;
using Loft.Common.DTOs;

namespace CartService.Mappings;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<Cart, CartDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));

        CreateMap<CartItem, CartItemDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}