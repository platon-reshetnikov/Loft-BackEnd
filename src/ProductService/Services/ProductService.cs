using AutoMapper;
using AutoMapper.QueryableExtensions;
using Loft.Common.DTOs;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Entities;


namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly ProductDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(ProductDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // Получение всех продуктов с пагинацией и фильтрацией
    public async Task<IEnumerable<ProductDTO>> GetAllProducts(int page = 1, int pageSize = 20,
        long? categoryId = null, long? sellerId = null)
    {
        var query = _context.Products.AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (sellerId.HasValue)
            query = query.Where(p => p.SellerId == sellerId.Value);

        return await query
            .OrderByDescending(p => p.DateAdded)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ProductDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // Получение продукта по Id
    public async Task<ProductDTO?> GetProductById(long productId)
    {
        var product = await _context.Products.FindAsync(productId);
        return product == null ? null : _mapper.Map<ProductDTO>(product);
    }

    // Создание нового продукта
    public async Task<ProductDTO> CreateProduct(ProductDTO productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        product.DateAdded = DateTime.UtcNow;
        product.Status = "Pending"; // по умолчанию на модерацию

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductDTO>(product);
    }

    // Обновление продукта
    public async Task<ProductDTO?> UpdateProduct(long productId, ProductDTO productDto)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return null;

        _mapper.Map(productDto, product);
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<ProductDTO>(product);
    }

    // Удаление продукта
    public async Task DeleteProduct(long productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    // Поиск по названию / описанию
    public async Task<IEnumerable<ProductDTO>> SearchProducts(string query, int page = 1, int pageSize = 20)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
            .OrderByDescending(p => p.DateAdded)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ProductDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // Обновление количества товара на складе
    public async Task UpdateStock(long productId, int newQuantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            product.StockQuantity = newQuantity;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}