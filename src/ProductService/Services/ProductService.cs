using AutoMapper;
using Loft.Common.DTOs;
using Loft.Common.Enums;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Entities;

namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly ProductDbContext _db; 
    private readonly IMapper _mapper;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ProductDbContext db, IMapper mapper, IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
    {
        _db = db;
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        
    }
    public async Task<PagedResultFilterDto<ProductDto>> GetAllProducts(ProductFilterDto filter)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.AttributeValues).ThenInclude(av => av.Attribute)
            .Include(p => p.MediaFiles)
            .Include(p => p.Comments).ThenInclude(c => c.MediaFiles)
            .Where(p => p.Status == ModerationStatus.Approved)
            .AsQueryable();

        if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.SellerId.HasValue && filter.SellerId.Value > 0)
            query = query.Where(p => p.IdUser == filter.SellerId.Value);

        if (filter.MinPrice.HasValue && filter.MinPrice.Value > 0)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue && filter.MaxPrice.Value > 0)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = $"%{filter.Search.Trim()}%";

            query = query.Where(p =>
                EF.Functions.ILike(p.Name, search)
            );
        }

        if (filter.AttributeFilters != null && filter.AttributeFilters.Any())
        {
            foreach (var af in filter.AttributeFilters)
            {
                if (af.AttributeId > 0 && !string.IsNullOrWhiteSpace(af.Value))
                {
                    query = query.Where(p => p.AttributeValues
                        .Any(av => av.AttributeId == af.AttributeId && av.Value == af.Value));
                }
            }
        }

        var totalCount = await query.CountAsync();
        var page = filter.Page > 0 ? filter.Page : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 20;
        var skip = (page - 1) * pageSize;

        var products = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PagedResultFilterDto<ProductDto>
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            Page = page,
            PageSize = pageSize,
            Items = _mapper.Map<IEnumerable<ProductDto>>(products)
        };
    }

    public async Task<IEnumerable<ProductDto>> GetAllMyProducts(long userId)
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.AttributeValues).ThenInclude(av => av.Attribute)
            .Include(p => p.MediaFiles)
            .Include(p => p.Comments).ThenInclude(c => c.MediaFiles)
            .Where(p => p.IdUser == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductById(int productId, bool isModerator)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.AttributeValues).ThenInclude(av => av.Attribute)
            .Include(p => p.MediaFiles)
            .Include(p => p.Comments).ThenInclude(c => c.MediaFiles)
            .AsQueryable();

        if (!isModerator)
        {
            query = query.Where(p => p.Status == ModerationStatus.Approved);
        }

        var product = await query.FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return null;

        await _db.Products
            .Where(p => p.Id == productId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));

        product.ViewCount++;

        return _mapper.Map<ProductDto?>(product);
    }

    public async Task<ProductDto> CreateProduct(ProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateProduct(int productId, ProductDto productDto)
    {
        var product = await _db.Products
            .Include(p => p.AttributeValues)
            .Include(p => p.MediaFiles)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null) return null;

        _mapper.Map(productDto, product);
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteProduct(int productId)
    {
        var product = await _db.Products
            .Include(p => p.MediaFiles)
            .Include(p => p.Comments)
                .ThenInclude(c => c.MediaFiles) 
            .Include(p => p.AttributeValues)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return;

        var commentMedia = product.Comments.SelectMany(c => c.MediaFiles).ToList();
        if (commentMedia.Any())
            _db.MediaFiles.RemoveRange(commentMedia);

        if (product.Comments.Any())
            _db.Comments.RemoveRange(product.Comments);

        if (product.MediaFiles.Any())
            _db.MediaFiles.RemoveRange(product.MediaFiles);

        if (product.AttributeValues.Any())
            _db.ProductAttributeValues.RemoveRange(product.AttributeValues);

        _db.Products.Remove(product);

        await _db.SaveChangesAsync();
    }

    public async Task<ProductDto?> UpdateProductQuantity(int productId, int newQuantity)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product == null) return null;

        if (product.Type == ProductType.Digital)
        {
            _logger.LogWarning($"Cannot update quantity for digital product {productId}");
            return _mapper.Map<ProductDto>(product);
        }

        product.Quantity = newQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        
        _logger.LogInformation($"Updated quantity for product {productId} to {newQuantity}");
        
        return _mapper.Map<ProductDto>(product);
    }
    
    public async Task<IEnumerable<CategoryDto>> GetAllCategories()
    {
        var categories = await _db.Categories
            .Include(c => c.CategoryAttributes).ThenInclude(ca => ca.Attribute)
            .Include(c => c.Products)
            .AsNoTracking()
            .ToListAsync();

        var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
        var lookup = categoryDtos.ToDictionary(c => c.Id);

        List<CategoryDto> rootCategories = new();

        foreach (var category in categoryDtos)
        {
            if (category.ParentCategoryId == null)
            {
                rootCategories.Add(category);
            }
            else if (lookup.TryGetValue(category.ParentCategoryId.Value, out var parent))
            {
                parent.SubCategories ??= new List<CategoryDto>();
                parent.SubCategories.Add(category);
            }
        }

        return rootCategories;
    }
    
    public async Task<CategoryDto?> GetCategoryById(int categoryId)
    {
        var category = await _db.Categories
            .Include(c => c.CategoryAttributes).ThenInclude(ca => ca.Attribute)
            .Include(c => c.Products)
            .Include(c => c.SubCategories)
            .ThenInclude(sc => sc.CategoryAttributes).ThenInclude(ca => ca.Attribute)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        return _mapper.Map<CategoryDto?>(category);
    }

    public async Task<CategoryDto> CreateCategory(CategoryDto dto)
    {

        var category = new Category
        {
            Name = dto.Name,
            ImageUrl = dto.ImageUrl,
            ParentCategoryId = dto.ParentCategoryId,
            Status = 0,
            Type = dto.Type,
            ViewCount = 0
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }

    public async Task<CategoryDto?> UpdateCategory(int categoryId, CategoryDto categoryDto)
    {
        var category = await _db.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null) return null;

        _mapper.Map(categoryDto, category);

        if (categoryDto.SubCategories != null)
        {
            foreach (var subDto in categoryDto.SubCategories)
            {
                var existingSub = category.SubCategories.FirstOrDefault(sc => sc.Id == subDto.Id);
                if (existingSub != null)
                {
                    _mapper.Map(subDto, existingSub);
                }
                else
                {
                    var newSub = _mapper.Map<Category>(subDto);
                    newSub.ParentCategoryId = category.Id;
                    _db.Categories.Add(newSub);
                }
            }
        }

        await _db.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteCategory(int categoryId)
    {
        var category = await _db.Categories.FindAsync(categoryId);
        if (category != null)
        {
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<AttributeDto>> GetAllAttributes()
    {
        var attributes = await _db.AttributeEntity
            .AsNoTracking() // для ускорения — данные только читаются
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<AttributeDto>>(attributes);
    }

    public async Task<AttributeDto?> GetAttributeById(int attributeId)
    {
        var attribute = await _db.AttributeEntity.FindAsync(attributeId);
        return _mapper.Map<AttributeDto?>(attribute);
    }

    public async Task<AttributeDto> CreateAttribute(AttributeDto attributeDto)
    {
        var attribute = _mapper.Map<AttributeEntity>(attributeDto);
        _db.AttributeEntity.Add(attribute);
        await _db.SaveChangesAsync();
        return _mapper.Map<AttributeDto>(attribute);
    }

    public async Task<AttributeDto?> UpdateAttribute(int attributeId, AttributeDto attributeDto)
    {
        var attribute = await _db.AttributeEntity.FindAsync(attributeId);
        if (attribute == null) return null;

        _mapper.Map(attributeDto, attribute);
        await _db.SaveChangesAsync();
        return _mapper.Map<AttributeDto>(attribute);
    }

    public async Task DeleteAttribute(int attributeId)
    {
        var attribute = await _db.AttributeEntity
            .Include(a => a.AttributeValues)
            .Include(a => a.CategoryAttributes)
            .FirstOrDefaultAsync(a => a.Id == attributeId);

        if (attribute != null)
        {
            if (attribute.AttributeValues.Any())
                _db.ProductAttributeValues.RemoveRange(attribute.AttributeValues);

            if (attribute.CategoryAttributes.Any())
                _db.CategoryAttributes.RemoveRange(attribute.CategoryAttributes);
            _db.AttributeEntity.Remove(attribute);
            
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task<CategoryAttributeDto> AssignAttributeToCategory(int categoryId, int attributeId, bool isRequired, int orderIndex)
    {
        var existing = await _db.CategoryAttributes
            .FirstOrDefaultAsync(ca => ca.CategoryId == categoryId && ca.AttributeId == attributeId);

        if (existing != null)
        {
            existing.IsRequired = isRequired;
            existing.OrderIndex = orderIndex;
            await _db.SaveChangesAsync();
            return _mapper.Map<CategoryAttributeDto>(existing);
        }

        var categoryAttribute = new CategoryAttribute
        {
            CategoryId = categoryId,
            AttributeId = attributeId,
            IsRequired = isRequired,
            OrderIndex = orderIndex,
            Status = ModerationStatus.Pending
        };

        _db.CategoryAttributes.Add(categoryAttribute);
        await _db.SaveChangesAsync();
        return _mapper.Map<CategoryAttributeDto>(categoryAttribute);
    }

    public async Task RemoveAttributeFromCategory(int categoryId, int attributeId)
    {
        var categoryAttribute = await _db.CategoryAttributes
            .FirstOrDefaultAsync(ca => ca.CategoryId == categoryId && ca.AttributeId == attributeId);

        if (categoryAttribute != null)
        {
            _db.CategoryAttributes.Remove(categoryAttribute);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<CategoryAttributeDto>> GetCategoryAttributes(int categoryId)
    {
        var attributes = await _db.CategoryAttributes
            .Include(ca => ca.Attribute)
            .Where(ca => ca.CategoryId == categoryId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CategoryAttributeDto>>(attributes);
    }
    
    public async Task<IEnumerable<ProductDto>> GetProductsByModerationStatus(ModerationStatus status)
    {
        var products = await _db.Products
            .Where(p => p.Status == status)
            .Include(p => p.Category)
            .Include(p => p.AttributeValues).ThenInclude(av => av.Attribute)
            .Include(p => p.MediaFiles)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> UpdateProductModerationStatus(int productId, ModerationStatus status)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product == null) return null;

        product.Status = status;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }
}
