using AutoMapper;
using CategoryService.Entities;
using Loft.Common.DTOs;

namespace CategoryService.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.CategoryImageUrl, opt => opt.MapFrom(src => src.CategoryImageUrl));
    }
}