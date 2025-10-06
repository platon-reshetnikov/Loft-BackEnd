using CategoryService.Services;
using Loft.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CategoryService.Controllers
{
        [ApiController]
        [Route("api/categories")]
        public class CategoriesController : ControllerBase
        {
            private readonly ICategoryService _categoryService;

            public CategoriesController(ICategoryService categoryService)
            {
                _categoryService = categoryService;
            }

            [HttpGet]
            public async Task<IActionResult> GetCategories()
            {
                var categories = await _categoryService.GetAllCategories();
                return Ok(categories);
            }

            [HttpGet("{categoryId}")]
            public async Task<IActionResult> GetCategoryById(int categoryId)
            {
                var category = await _categoryService.GetCategoryById(categoryId);
                if (category == null)
                    return NotFound();
                return Ok(category);
            }

            [HttpGet("{parentId}/subcategories")]
            public async Task<IActionResult> GetSubcategories(int parentId)
            {
                var subcategories = await _categoryService.GetSubcategories(parentId);
                return Ok(subcategories);
            }

            [HttpPost]
            public async Task<IActionResult> CreateCategory([FromBody] CategoryDto category)
            {
                try
                {
                    var createdCategory = await _categoryService.CreateCategory(category);
                    return CreatedAtAction(nameof(GetCategoryById), new { categoryId = createdCategory.Id }, createdCategory);
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpPut("{categoryId}")]
            public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryDto category)
            {
                try
                {
                    var updatedCategory = await _categoryService.UpdateCategory(categoryId, category);
                    if (updatedCategory == null)
                        return NotFound();
                    return Ok(updatedCategory);
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpDelete("{categoryId}")]
            public async Task<IActionResult> DeleteCategory(int categoryId)
            {
                try
                {
                    await _categoryService.DeleteCategory(categoryId);
                    return NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpGet("attributes")]
            public async Task<IActionResult> GetAllAttributes()
            {
                var attributes = await _categoryService.GetAllAtributs();
                return Ok(attributes);
            }

            [HttpPost("attributes")]
            public async Task<IActionResult> CreateAttribute([FromBody] AttributeDto attribute)
            {
                try
                {
                    var createdAttribute = await _categoryService.CreateAtribut(attribute);
                    return CreatedAtAction(nameof(GetAllAttributes), new { attributeId = createdAttribute.Id }, createdAttribute);
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpPut("attributes/{attributeId}")]
            public async Task<IActionResult> UpdateAttribute(int attributeId, [FromBody] AttributeDto attribute)
            {
                try
                {
                    var updatedAttribute = await _categoryService.UpdateAtribut(attributeId, attribute);
                    if (updatedAttribute == null)
                        return NotFound();
                    return Ok(updatedAttribute);
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpDelete("attributes/{attributeId}")]
            public async Task<IActionResult> DeleteAttribute(int attributeId)
            {
                try
                {
                    await _categoryService.DeleteAtribut(attributeId);
                    return NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpGet("product-attributes")]
            public async Task<IActionResult> GetAllProductAttributes()
            {
                var productAttributes = await _categoryService.GetAllProductAtributs();
                return Ok(productAttributes);
            }

            [HttpPost("product-attributes")]
            public async Task<IActionResult> CreateProductAttribute([FromBody] ProductAttributeDto productAttribute)
            {
                try
                {
                    var createdProductAttribute = await _categoryService.CreateProductAtribut(productAttribute);
                    return CreatedAtAction(nameof(GetAllProductAttributes), new { productAttributeId = createdProductAttribute.Id }, createdProductAttribute);
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpPut("product-attributes/{productAttributeId}")]
            public async Task<IActionResult> UpdateProductAttribute(int productAttributeId, [FromBody] ProductAttributeDto productAttribute)
            {
                try
                {
                    var updatedProductAttribute = await _categoryService.UpdateProductAtribut(productAttributeId, productAttribute);
                    if (updatedProductAttribute == null)
                        return NotFound();
                    return Ok(updatedProductAttribute);
                }
                catch (ValidationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpDelete("product-attributes/{productAttributeId}")]
            public async Task<IActionResult> DeleteProductAttribute(int productAttributeId)
            {
                try
                {
                    await _categoryService.DeleteProductAtribut(productAttributeId);
                    return NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }
        }
}