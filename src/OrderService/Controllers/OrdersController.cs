using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetOrders()
        {
            return Ok(new[] { "Order1", "Order2" });
        }
    }
}