using Loft.Common.DTOs;
using Loft.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Services;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : BaseController
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterDto filter)
        {
            var result = await _service.GetAllProducts(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            long? userId = GetUserId();
            bool isModerator = false;

            if (userId != null)
            {
                isModerator = IsModerator();

                var productTmp = await _service.GetProductById(id, true);
                if (productTmp == null) return NotFound();

                if (productTmp.IdUser == userId)
                {
                    isModerator = true;
                }
            }

            var product = await _service.GetProductById(id, isModerator);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpGet("myproducts")]
        public async Task<IActionResult> GetMyProducts()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var products = await _service.GetAllMyProducts(userId.Value);

            return Ok(products);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ProductDto productDto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            productDto.IdUser = (int)userId;
            productDto.Status = Loft.Common.Enums.ModerationStatus.Pending;

            var product = await _service.CreateProduct(productDto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto productDto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var product = await _service.GetProductById(id, true);
            if (product == null) return NotFound();

            if (product.IdUser != userId)
                return Forbid();

            productDto.IdUser = product.IdUser;
            productDto.Status = Loft.Common.Enums.ModerationStatus.Pending;

            var updated = await _service.UpdateProduct(id, productDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var product = await _service.GetProductById(id, true);
            if (product == null) return NotFound();

            if (product.IdUser != userId)
                return Forbid();

            await _service.DeleteProduct(id);
            return NoContent();
        }

        [HttpPut("{id}/quantity")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateQuantityRequest request)
        {
            var product = await _service.GetProductById(id, true);
            if (product == null) return NotFound();

            if (product.Type == ProductType.Digital)
            {
                return BadRequest(new { error = "Cannot update quantity for digital products" });
            }

            if (request.Quantity < 0)
            {
                return BadRequest(new { error = "Quantity cannot be negative" });
            }

            var updated = await _service.UpdateProductQuantity(id, request.Quantity);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpPut("{id}/reduce-quantity")]
        public async Task<IActionResult> ReduceQuantity(int id, [FromBody] ReduceQuantityRequest request)
        {
            var product = await _service.GetProductById(id, true);
            if (product == null) return NotFound();

            if (product.Type == ProductType.Digital)
            {
                return Ok(new { message = "Digital product - quantity not changed", product });
            }

            if (product.Quantity < request.Quantity)
            {
                return BadRequest(new { error = $"Insufficient quantity. Available: {product.Quantity}, Requested: {request.Quantity}" });
            }

            var newQuantity = product.Quantity - request.Quantity;
            var updated = await _service.UpdateProductQuantity(id, newQuantity);
            if (updated == null) return NotFound();

            return Ok(updated);
        }
    }

    public class UpdateQuantityRequest
    {
        public int Quantity { get; set; }
    }

    public class ReduceQuantityRequest
    {
        public int Quantity { get; set; }
    }
}
