using Loft.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Services;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly IProductService _service;

        public CategoryController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetCategoryById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CategoryDto categoryDto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            Console.WriteLine("=== JWT Claims Start ===");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }
            Console.WriteLine("=== JWT Claims End ===");

            if (!IsModerator()) return Forbid();

            var category = await _service.CreateCategory(categoryDto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto categoryDto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (!IsModerator()) return Forbid();

            var updated = await _service.UpdateCategory(id, categoryDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (!IsModerator()) return Forbid();

            await _service.DeleteCategory(id);
            return NoContent();
        }

        [HttpPost("{id}/attributes")]
        [Authorize] 
        public async Task<IActionResult> AssignAttribute(int id, [FromQuery] int attributeId, [FromQuery] bool isRequired, [FromQuery] int orderIndex)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (!IsModerator()) return Forbid();

            var categoryAttribute = await _service.AssignAttributeToCategory(id, attributeId, isRequired, orderIndex);
            return Ok(categoryAttribute);
        }

        [HttpDelete("{id}/attributes/{attributeId}")]
        [Authorize]
        public async Task<IActionResult> RemoveAttribute(int id, int attributeId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (!IsModerator()) return Forbid();

            await _service.RemoveAttributeFromCategory(id, attributeId);
            return NoContent();
        }

        [HttpGet("{id}/attributes")]
        public async Task<IActionResult> GetAttributes(int id)
        {
            var attributes = await _service.GetCategoryAttributes(id);
            return Ok(attributes);
        }
    }
}
