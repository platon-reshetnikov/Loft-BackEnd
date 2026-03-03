using AutoMapper;
using Loft.Common.DTOs;
using ProductService.Entities;

namespace ProductService.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.AttributeValues, opt => opt.MapFrom(src => src.AttributeValues))
                .ForMember(dest => dest.MediaFiles, opt => opt.MapFrom(src => src.MediaFiles))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ReverseMap(); 

            CreateMap<ProductAttributeValue, ProductAttributeValueDto>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ReverseMap();

            CreateMap<MediaFile, MediaFileDto>().ReverseMap();

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.MediaFiles, opt => opt.MapFrom(src => src.MediaFiles))
                .ReverseMap();

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.CategoryAttributes))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
                .ReverseMap();

            CreateMap<CategoryAttribute, CategoryAttributeDto>()
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.Attribute.Name))
                .ReverseMap();

            CreateMap<AttributeEntity, AttributeDto>().ReverseMap();
        }
    }
}
