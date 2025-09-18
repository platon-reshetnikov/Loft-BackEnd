using Microsoft.AspNetCore.Mvc;

namespace CategoryService.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(new[] { "Category1", "Category2" });
        }
    }
}