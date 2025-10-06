using Loft.Common.DTOs;

namespace CategoryService.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategories(); // возвращает все категории 
    Task<CategoryDto?> GetCategoryById(int categoryId); // возвращает категорию по ID
    Task<IEnumerable<CategoryDto>> GetSubcategories(int parentId); // возвращает подкатегории для заданной категории
    Task<CategoryDto> CreateCategory(CategoryDto category); // создает новую категорию
    Task<CategoryDto?> UpdateCategory(int categoryId, CategoryDto category);   // обновляет существующую категорию
    Task DeleteCategory(int categoryId);   // удаляет категорию по ID


    Task<AttributeDto> CreateAtribut(AttributeDto Atribut); // создает новую атрибут
    Task<AttributeDto?> UpdateAtribut(int AtributId, AttributeDto Atribut);   // обновляет существующую категорию
    Task DeleteAtribut(int AtributId);   // удаляет атрибут по ID
    Task<IEnumerable<AttributeDto>> GetAllAtributs(); // возвращает все атрибуты


    Task<ProductAttributeDto> CreateProductAtribut(ProductAttributeDto ProductAtribut); // создает новую атрибут
    Task<ProductAttributeDto?> UpdateProductAtribut(int ProductAtributId, ProductAttributeDto ProductAtribut);   // обновляет существующую категорию
    Task DeleteProductAtribut(int ProductAtributId);   // удаляет атрибут по ID
    Task<IEnumerable<ProductAttributeDto>> GetAllProductAtributs(); // возвращает все атрибуты
}