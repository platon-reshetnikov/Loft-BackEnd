namespace UserService.Services;

using Loft.Common.DTOs;

public interface ITokenService
{
    string GenerateToken(UserDTO user);
}
