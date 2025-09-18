using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/carts")]
    public class CartsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCarts()
        {
            return Ok(new[] { "Cart1", "Cart2" });
        }
    }
}