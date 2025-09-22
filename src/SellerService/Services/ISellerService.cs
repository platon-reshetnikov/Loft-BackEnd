using Loft.Common.DTOs;

namespace SellerService.Services;

public interface ISellerService
{
    Task<IEnumerable<SellerDTO>> GetAllSellers(int page = 1,int pageSize = 20);
    Task<SellerDTO?> GetSellerById(long sellerId);
    Task<SellerDTO?> GetSellerByUserId(long userId);
    Task<SellerDTO> CreateSeller(SellerDTO seller);
    Task<SellerDTO?> UpdateSeller(long sellerId, SellerDTO seller);
    Task DeleteSeller(long sellerId);
    Task<decimal> GetSellerBalance(long sellerId);
    Task AdjustSellerBalance(long sellerId, decimal amount,string reason);
    
    /*
     * Примечания: GetSellerByUserId — удобно, когда у вас отдельная таблица продавцов.
     */
    
}