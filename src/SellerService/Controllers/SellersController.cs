using Microsoft.AspNetCore.Mvc;

namespace SellerService.Controllers
{
    [ApiController]
    [Route("api/sellers")]
    public class SellersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSellers()
        {
            return Ok(new[] { "Seller1", "Seller2" });
        }
    }
}