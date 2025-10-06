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
        [HttpPost("add")]
        public async Task<IActionResult> addProduct([FromBody] ProductDTO productDto)
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

        [HttpGet("approved-count")] // Получение количества одобренных продуктов
        public async Task<ActionResult<int>> GetApprovedProductsCount()
        {
            var count = await _productService.GetApprovedProductsCount();
            return Ok(count);
        }

        // Получение всех продуктов
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var products = await _productService.GetAllProducts(page, pageSize);
            return Ok(products);
        }

        // 🔍 Поиск продуктов
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProducts(
            [FromQuery] string? text,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var products = await _productService.SearchProducts(text, minPrice, maxPrice);
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

        // Обновление количества товара
        [HttpPatch("{id:long}/stock")]
        public async Task<IActionResult> UpdateStock(long id, [FromQuery] int quantity)
        {
            await _productService.UpdateStock(id, quantity);
            return NoContent();
        }

        // Добавление комментария к продукту
        [HttpPost("AddComent")]
        public async Task<IActionResult> AddComent(long id, [FromQuery] string txt)
        {
            await _productService.AddComent(id, txt);
            return Ok();
        }
    }
}