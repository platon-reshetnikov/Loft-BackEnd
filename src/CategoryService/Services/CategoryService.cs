using AutoMapper;
using CategoryService.Data;
using CategoryService.Entities;
using Loft.Common.DTOs;
using Loft.Common.Enums;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using System.ComponentModel.DataAnnotations;

namespace CategoryService.Services;

public class CategoryService : ICategoryService
{
    private readonly CategoryDbContext _context;
    private readonly IMapper _mapper;
    public CategoryService(CategoryDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategories()
    {
        var categories = await _context.Category
            .Include(c => c.Attributes)
            .ToListAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetCategoryById(int categoryId)
    {
        var category = await _context.Category
            .Include(c => c.Attributes)
            .FirstOrDefaultAsync(c => c.Id == categoryId);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetSubcategories(int parentId)
    {
        var subcategories = await _context.Category
            .Include(c => c.Attributes)
            .Where(c => c.ParentId == parentId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(subcategories);
    }

    public async Task<CategoryDto> CreateCategory(CategoryDto category)
    {
        var categoryEntity = _mapper.Map<Category>(category);

        if (category.ParentId.HasValue)
        {
            var parent = await _context.Category.FindAsync(category.ParentId);
            if (parent == null)
                throw new ValidationException("Parent category not found.");
        }

        _context.Category.Add(categoryEntity);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(categoryEntity);
    }

    public async Task<CategoryDto?> UpdateCategory(int categoryId, CategoryDto category)
    {
        var categoryEntity = await _context.Category
            .Include(c => c.Attributes)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (categoryEntity == null)
            return null;

        if (category.AttributeIds.Count > 10)
            throw new ValidationException("Category cannot have more than 10 attributes.");

        _mapper.Map(category, categoryEntity);

        if (category.ParentId.HasValue)
        {
            var parent = await _context.Category.FindAsync(category.ParentId);
            if (parent == null)
                throw new ValidationException("Parent category not found.");
        }

        categoryEntity.Attributes.Clear();
        if (category.AttributeIds.Any())
        {
            var attributes = await _context.Atribut
                .Where(a => category.AttributeIds.Contains(a.Id))
                .ToListAsync();

            if (attributes.Count != category.AttributeIds.Count)
                throw new ValidationException("One or more attributes not found.");

            categoryEntity.Attributes = attributes;
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(categoryEntity);
    }

    public async Task DeleteCategory(int categoryId)
    {
        var category = await _context.Category
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new KeyNotFoundException("Category not found.");

        if (category.SubCategories.Any())
            throw new ValidationException("Cannot delete category with subcategories.");

        _context.Category.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<AttributeDto> CreateAtribut(AttributeDto atribut)
    {
        if (atribut.Type == AttributeType.List && string.IsNullOrEmpty(atribut.ListOptions))
            throw new ValidationException("List attributes must have ListOptions.");

        var attributeEntity = _mapper.Map<Atribut>(atribut);

        _context.Atribut.Add(attributeEntity);
        await _context.SaveChangesAsync();

        return _mapper.Map<AttributeDto>(attributeEntity);
    }

    public async Task<AttributeDto?> UpdateAtribut(int atributId, AttributeDto atribut)
    {
        var attributeEntity = await _context.Atribut.FindAsync(atributId);
        if (attributeEntity == null)
            return null;

        if (atribut.Type == AttributeType.List && string.IsNullOrEmpty(atribut.ListOptions))
            throw new ValidationException("List attributes must have ListOptions.");

        _mapper.Map(atribut, attributeEntity);

        await _context.SaveChangesAsync();
        return _mapper.Map<AttributeDto>(attributeEntity);
    }

    public async Task DeleteAtribut(int atributId)
    {
        var attribute = await _context.Atribut
            .Include(a => a.Categories)
            .FirstOrDefaultAsync(a => a.Id == atributId);

        if (attribute == null)
            throw new KeyNotFoundException("Attribute not found.");

        if (attribute.Categories.Any())
            throw new ValidationException("Cannot delete attribute in use by categories or products.");

        _context.Atribut.Remove(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AttributeDto>> GetAllAtributs()
    {
        var attributes = await _context.Atribut.ToListAsync();
        return _mapper.Map<IEnumerable<AttributeDto>>(attributes);
    }

    public async Task<ProductAttributeDto> CreateProductAtribut(ProductAttributeDto productAtribut)
    {
        var attribute = await _context.Atribut.FindAsync(productAtribut.AttributeId);
        if (attribute == null)
            throw new ValidationException("Attribute not found.");


        if (attribute.Type == AttributeType.Number && !double.TryParse(productAtribut.Value, out _))
            throw new ValidationException("Value must be a number for numeric attributes.");

        if (attribute.Type == AttributeType.List)
        {
            var options = attribute.ListOptions?.Split(',').Select(o => o.Trim()).ToList();
            if (options != null && !options.Contains(productAtribut.Value))
                throw new ValidationException("Value must be one of the predefined list options.");
        }

        var productAttributeEntity = _mapper.Map<ProductAttribute>(productAtribut);

        _context.ProductAttribute.Add(productAttributeEntity);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductAttributeDto>(productAttributeEntity);
    }

    public async Task<ProductAttributeDto?> UpdateProductAtribut(int productAtributId, ProductAttributeDto productAtribut)
    {
        var productAttributeEntity = await _context.ProductAttribute.FindAsync(productAtributId);
        if (productAttributeEntity == null)
            return null;

        var attribute = await _context.Atribut.FindAsync(productAtribut.AttributeId);
        if (attribute == null)
            throw new ValidationException("Attribute not found.");

        if (attribute.Type == AttributeType.Number && !double.TryParse(productAtribut.Value, out _))
            throw new ValidationException("Value must be a number for numeric attributes.");

        if (attribute.Type == AttributeType.List)
        {
            var options = attribute.ListOptions?.Split(',').Select(o => o.Trim()).ToList();
            if (options != null && !options.Contains(productAtribut.Value))
                throw new ValidationException("Value must be one of the predefined list options.");
        }

        _mapper.Map(productAtribut, productAttributeEntity);

        await _context.SaveChangesAsync();
        return _mapper.Map<ProductAttributeDto>(productAttributeEntity);
    }

    public async Task DeleteProductAtribut(int productAtributId)
    {
        var productAttribute = await _context.ProductAttribute.FindAsync(productAtributId);
        if (productAttribute == null)
            throw new KeyNotFoundException("Product attribute not found.");

        _context.ProductAttribute.Remove(productAttribute);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductAttributeDto>> GetAllProductAtributs()
    {
        var productAttributes = await _context.ProductAttribute.ToListAsync();
        return _mapper.Map<IEnumerable<ProductAttributeDto>>(productAttributes);
    }
}