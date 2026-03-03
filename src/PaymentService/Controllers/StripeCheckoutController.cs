using Microsoft.AspNetCore.Mvc;
using PaymentService.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/stripe")]
public class StripeCheckoutController : ControllerBase
{
    private readonly IStripeCheckoutService _checkoutService;
    private readonly ILogger<StripeCheckoutController> _logger;

    public StripeCheckoutController(
        IStripeCheckoutService checkoutService,
        ILogger<StripeCheckoutController> logger)
    {
        _checkoutService = checkoutService;
        _logger = logger;
    }
    
    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
    {
        try
        {
            var sessionId = await _checkoutService.CreateCheckoutSessionAsync(
                request.OrderId,
                request.Amount,
                request.SuccessUrl,
                request.CancelUrl
            );

            var session = await _checkoutService.GetCheckoutSessionAsync(sessionId);

            return Ok(new
            {
                sessionId = sessionId,
                url = session.Url,
                publishableKey = Request.HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Stripe:PublishableKey"]
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session for order {OrderId}", request.OrderId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSession(string sessionId)
    {
        try
        {
            var session = await _checkoutService.GetCheckoutSessionAsync(sessionId);
            
            return Ok(new
            {
                id = session.Id,
                paymentStatus = session.PaymentStatus,
                customerEmail = session.CustomerEmail,
                amountTotal = session.AmountTotal,
                currency = session.Currency,
                paymentIntentId = session.PaymentIntentId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving session {SessionId}", sessionId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        try
        {
            var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();

            var success = await _checkoutService.HandleWebhookAsync(payload, signature);

            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Webhook validation failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling webhook");
            return StatusCode(500);
        }
    }
}

public class CreateCheckoutSessionRequest
{
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string SuccessUrl { get; set; } = "";
    public string CancelUrl { get; set; } = "";
}
