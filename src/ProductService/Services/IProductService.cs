using Loft.Common.DTOs;
using Loft.Common.Enums;

namespace ProductService.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetAllProducts(int page, int pageSize); // получение всех продуктов
    Task<int> GetApprovedProductsCount(); // получение количества одобренных продуктов
    Task<ProductDTO?> GetProductById(long productId); // получение продукта по Id
    Task<ProductDTO> CreateProduct(ProductDTO product); // создание нового продукта
    Task<ProductDTO?> UpdateProduct(long productId, ProductDTO product); // обновление продукта
    Task DeleteProduct(long productId); // удаление продукта
    Task<IEnumerable<ProductDTO>> SearchProducts(string? searchText, decimal? minPrice, decimal? maxPrice); // поиск продуктов по критериям
    Task UpdateStock(long productId, int newQuantity);  // обновление количества товара на складе
    Task AddComent(long productId, string newComent);    // добавление комментария к продукту
}