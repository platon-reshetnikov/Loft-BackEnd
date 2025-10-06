using AutoMapper;
using CategoryService.Entities;
using Loft.Common.DTOs;

namespace CategoryService.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        // Category to CategoryDto
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.ViewCount))
            .ForMember(dest => dest.AttributeIds, opt => opt.MapFrom(src => src.Attributes.Select(a => a.Id).ToList()));

        // CategoryDto to Category
        CreateMap<CategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for creating new entities
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.ViewCount))
            .ForMember(dest => dest.Attributes, opt => opt.Ignore()); // Attributes are handled in service layer

        // Atribut to AttributeDto
        CreateMap<Atribut, AttributeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TypeName))
            .ForMember(dest => dest.ListOptions, opt => opt.MapFrom(src => src.ListOptions))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        // AttributeDto to Atribut
        CreateMap<AttributeDto, Atribut>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TypeName))
            .ForMember(dest => dest.ListOptions, opt => opt.MapFrom(src => src.ListOptions))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        // ProductAttribute to ProductAttributeDto
        CreateMap<ProductAttribute, ProductAttributeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AttributeId, opt => opt.MapFrom(src => src.AttributeId))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));

        // ProductAttributeDto to ProductAttribute
        CreateMap<ProductAttributeDto, ProductAttribute>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AttributeId, opt => opt.MapFrom(src => src.AttributeId))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Attribute, opt => opt.Ignore());
    }
}