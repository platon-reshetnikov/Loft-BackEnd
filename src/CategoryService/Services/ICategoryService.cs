using Loft.Common.DTOs;

namespace CategoryService.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetAllCategories(int page = 1, int pageSize = 20);
    Task<CategoryDTO?> GetCategoryById(long categoryId);
    Task<IEnumerable<CategoryDTO>> GetSubcategories(long parentId);
    Task<CategoryDTO> CreateCategory(CategoryDTO category);
    Task<CategoryDTO?> UpdateCategory(long categoryId, CategoryDTO category);
    Task DeleteCategory(long categoryId);
    Task<bool> CategoryExists(long categoryId);
    
    /*
     * Примечания: пагинация по умолчанию; GetCategoryById возвращает nullable если не найдено.
     */
}