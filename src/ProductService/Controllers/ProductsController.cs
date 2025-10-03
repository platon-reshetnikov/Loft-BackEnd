using AutoMapper;
using Loft.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using ProductService.Services;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // Создание продукта
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDto)
        {
            if (productDto == null)
                return BadRequest("Product data is null");

            var createdProduct = await _productService.CreateProduct(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        // Получение продукта по Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // Получение всех продуктов
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            [FromQuery] long? categoryId = null, [FromQuery] long? sellerId = null)
        {
            var products = await _productService.GetAllProducts(page, pageSize, categoryId, sellerId);
            return Ok(products);
        }

        // Обновление продукта
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] ProductDTO productDto)
        {
            if (productDto == null)
                return BadRequest("Product data is null");

            var updatedProduct = await _productService.UpdateProduct(id, productDto);
            if (updatedProduct == null)
                return NotFound();

            return Ok(updatedProduct);
        }

        // Удаление продукта
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }

        // Поиск по названию и описанию
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var products = await _productService.SearchProducts(query, page, pageSize);
            return Ok(products);
        }

        // Обновление количества товара
        [HttpPatch("{id:long}/stock")]
        public async Task<IActionResult> UpdateStock(long id, [FromQuery] int quantity)
        {
            await _productService.UpdateStock(id, quantity);
            return NoContent();
        }
    }
}