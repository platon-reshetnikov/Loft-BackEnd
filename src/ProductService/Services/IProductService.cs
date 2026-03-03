using Loft.Common.DTOs;
using Loft.Common.Enums;
using ProductService.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Services;

public interface IProductService
{
    Task<PagedResultFilterDto<ProductDto>> GetAllProducts(ProductFilterDto filter);
    Task<IEnumerable<ProductDto>> GetAllMyProducts(long userId);
    Task<ProductDto?> GetProductById(int productId,bool isModerator);
    Task<ProductDto> CreateProduct(ProductDto product);
    Task<ProductDto?> UpdateProduct(int productId, ProductDto product);
    Task DeleteProduct(int productId);
    Task<ProductDto?> UpdateProductQuantity(int productId, int newQuantity);
    Task<IEnumerable<CategoryDto>> GetAllCategories();
    Task<CategoryDto?> GetCategoryById(int categoryId);
    Task<CategoryDto> CreateCategory(CategoryDto category);
    Task<CategoryDto?> UpdateCategory(int categoryId, CategoryDto category);
    Task DeleteCategory(int categoryId);
    Task<IEnumerable<AttributeDto>> GetAllAttributes();
    Task<AttributeDto?> GetAttributeById(int attributeId);
    Task<AttributeDto> CreateAttribute(AttributeDto attribute);
    Task<AttributeDto?> UpdateAttribute(int attributeId, AttributeDto attribute);
    Task DeleteAttribute(int attributeId);
    Task<CategoryAttributeDto> AssignAttributeToCategory(int categoryId, int attributeId, bool isRequired, int orderIndex);
    Task RemoveAttributeFromCategory(int categoryId, int attributeId);
    Task<IEnumerable<CategoryAttributeDto>> GetCategoryAttributes(int categoryId);
    Task<IEnumerable<ProductDto>> GetProductsByModerationStatus(ModerationStatus status);
    Task<ProductDto?> UpdateProductModerationStatus(int productId, ModerationStatus status);
}
