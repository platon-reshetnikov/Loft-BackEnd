using AutoMapper;
using Loft.Common.DTOs;
using UserService.Entities;

namespace UserService.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.CanSell, opt => opt.MapFrom(src => src.CanSell))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.Join(' ', new[] { src.FirstName, src.LastName }.Where(s => !string.IsNullOrWhiteSpace(s)))));
    }
    
}