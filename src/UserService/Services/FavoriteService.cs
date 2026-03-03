using Loft.Common.DTOs;
using UserService.Data;
using Microsoft.EntityFrameworkCore;

namespace UserService.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly UserDbContext _context;

        public FavoriteService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<FavoritesListDto> GetFavoritesAsync(long userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return new FavoritesListDto
            {
                ProductIds = user?.FavoriteProductIds?.ToList() ?? new List<int>()
            };
        }

        public async Task AddFavoriteAsync(long userId, int productId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;

            var list = user.FavoriteProductIds?.ToList() ?? new List<int>();

            if (!list.Contains(productId))
            {
                list.Add(productId);
                user.FavoriteProductIds = list.ToArray();
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFavoriteAsync(long userId, int productId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;

            var list = user.FavoriteProductIds?.ToList() ?? new List<int>();

            if (list.Contains(productId))
            {
                list.Remove(productId);
                user.FavoriteProductIds = list.ToArray();
                await _context.SaveChangesAsync();
            }
        }
    }
}
