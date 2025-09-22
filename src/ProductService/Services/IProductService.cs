using Loft.Common.DTOs;
using Loft.Common.Enums;

namespace ProductService.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetAllProducts(int page = 1, int pageSize = 20,
        long? categoryId = null, long? sellerId = null) ;
    Task<ProductDTO?> GetProductById(long productId);
    Task<ProductDTO> CreateProduct(ProductDTO product);
    Task<ProductDTO?> UpdateProduct(long productId, ProductDTO product);
    Task DeleteProduct(long productId);
    Task<IEnumerable<ProductDTO>> SearchProducts(string query, int page = 1, int pageSize = 20);
    Task<bool> ReserveStock(long productId, int quantity);
    Task ReleaseStock(long productId, int quantity);
    Task UpdateStock(long productId, int newQuantity);
    
    
}