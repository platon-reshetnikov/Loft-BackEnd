using AutoMapper;
using AutoMapper.QueryableExtensions;
using Loft.Common.DTOs;
using Loft.Common.Enums;
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

    public async Task AddComent(long productId, string newComent)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            _context.CommentProduct.Add(new CommentProduct
            {
                Text = newComent,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow,
                Status = ModerationStatus.Pending // по умолчанию на модерацию
            });

            await _context.SaveChangesAsync();
        }
    }

    // Получение количества одобренных продуктов
    public async Task<int> GetApprovedProductsCount()
    {
        return await _context.Products
            .AsNoTracking()
            .CountAsync(p => p.Status == ModerationStatus.Approved);
    }


    // Получение продуктов с пагинацией одобренных модерацией
    public async Task<IEnumerable<ProductDTO>> GetAllProducts(int page = 1, int pageSize = 20)
    {
        if (page < 1)
            throw new ArgumentException("Page number must be greater than or equal to 1", nameof(page));
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than or equal to 1", nameof(pageSize));

        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Status == ModerationStatus.Approved)
            .Include(p => p.Image)
            .Include(p => p.Comments)
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
        //product.Status = ModerationStatus.Pending; // по умолчанию на модерацию

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

    public async Task<IEnumerable<ProductDTO>> SearchProducts(string? text, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();

        // Только одобренные товары
        query = query.Where(p => p.Status == ModerationStatus.Approved);

        // Поиск по названию и описанию
        if (!string.IsNullOrWhiteSpace(text))
        {
            string lowerText = text.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(lowerText) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerText)));
        }

        // Фильтрация по цене
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        // Получаем результат и мапим в DTO
        var products = await query
            .OrderByDescending(p => p.DateAdded)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductDTO>>(products);
    }
}