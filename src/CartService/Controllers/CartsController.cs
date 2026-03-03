using Microsoft.AspNetCore.Mvc;
using CartService.Services;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/carts")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllCarts();
            return Ok(carts);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCart(long customerId)
        {
            var cart = await _cartService.GetCartByCustomerId(customerId);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCartByCustomer(long customerId)
        {
            var cart = await _cartService.GetCartByCustomerId(customerId);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpGet("{cartId}/items")]
        public async Task<IActionResult> GetCartItems(long cartId)
        {
            var items = await _cartService.GetCartItems(cartId);
            return Ok(items);
        }

        [HttpPost("{customerId}/items")]
        public async Task<IActionResult> AddToCart(long customerId, [FromBody] AddItemRequest req)
        {
            if (req == null) return BadRequest();
            var cart = await _cartService.AddToCart(customerId, req.ProductId, req.Quantity);
            return Ok(cart);
        }

        [HttpPut("{customerId}/items")]
        public async Task<IActionResult> UpdateCartItem(long customerId, [FromBody] UpdateItemRequest req)
        {
            if (req == null) return BadRequest();
            
            try
            {
                var item = await _cartService.UpdateCartItem(customerId, req.ProductId, req.Quantity);
                if (item == null) return NotFound();
                return Ok(item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{customerId}/items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(long customerId, long productId)
        {
            await _cartService.RemoveFormCart(customerId, productId);
            return NoContent();
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> ClearCart(long customerId)
        {
            await _cartService.ClearCart(customerId);
            return NoContent();
        }

        [HttpPost("merge")]
        public async Task<IActionResult> MergeCarts([FromBody] MergeRequest req)
        {
            if (req == null) return BadRequest();
            await _cartService.MergeCarts(req.FromCustomerId, req.ToCustomerId);
            return NoContent();
        }

        public record AddItemRequest(long ProductId, int Quantity);
        public record UpdateItemRequest(long ProductId, int Quantity);
        public record MergeRequest(long FromCustomerId, long ToCustomerId);
    }
}
