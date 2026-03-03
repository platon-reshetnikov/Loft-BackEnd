using Loft.Common.DTOs;

namespace UserService.Services
{
    public interface IFavoriteService
    {
        Task<FavoritesListDto> GetFavoritesAsync(long userId);
        Task AddFavoriteAsync(long userId, int productId);
        Task RemoveFavoriteAsync(long userId, int productId);
    }
}
