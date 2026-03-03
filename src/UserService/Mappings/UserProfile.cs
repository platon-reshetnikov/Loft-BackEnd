using AutoMapper;
using Loft.Common.DTOs;
using UserService.Entities;

namespace UserService.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<UserDTO, User>();
        CreateMap<UserDTO, PublicUserDTO>();
    }
}
