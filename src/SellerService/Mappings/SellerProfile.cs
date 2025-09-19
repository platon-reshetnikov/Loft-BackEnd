using AutoMapper;
using Loft.Common.DTOs;
using SellerService.Entities;

namespace SellerService.Mappings;

public class SellerProfile : Profile
{
    public SellerProfile()
    {
        CreateMap<Seller, SellerDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.StoreName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.StoreLogoUrl, opt => opt.MapFrom(src => src.StoreLogoUrl));
    }
}