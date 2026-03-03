using Stripe; 

namespace PaymentService.Services.Providers;

public class RealStripeProvider : IPaymentProvider
{
    private readonly ILogger<RealStripeProvider> _logger;
    private readonly PaymentIntentService _paymentIntentService;
    private readonly RefundService _refundService;

    public Loft.Common.Enums.PaymentMethod SupportedMethod => Loft.Common.Enums.PaymentMethod.STRIPE;

    public RealStripeProvider(IConfiguration configuration, ILogger<RealStripeProvider> logger)
    {
        _logger = logger;
        
        var apiKey = configuration["Stripe:SecretKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Stripe:SecretKey is not configured");
        }
        
        StripeConfiguration.ApiKey = apiKey;

        _paymentIntentService = new PaymentIntentService();
        _refundService = new RefundService();

        _logger.LogInformation("[REAL STRIPE] Initialized with API key: {KeyPrefix}...", 
            apiKey.Substring(0, Math.Min(10, apiKey.Length)));
    }

    public async Task<string> CreatePaymentAsync(decimal amount, long orderId)
    {
        try
        {
            var amountInSmallestUnit = (long)(amount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInSmallestUnit,
                Currency = "usd",
                PaymentMethod = "pm_card_visa",
                PaymentMethodTypes = new List<string> { "card" },
                AutomaticPaymentMethods = null,
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", orderId.ToString() },
                    { "integration", "loft_payment_service" }
                }
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options);

            _logger.LogInformation(
                "[REAL STRIPE] Created PaymentIntent {PaymentIntentId} for order {OrderId}, amount {Amount} pence, status: {Status}", 
                paymentIntent.Id, orderId, amountInSmallestUnit, paymentIntent.Status);

            return paymentIntent.Id;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "[REAL STRIPE] Error creating payment intent: {ErrorMessage}", ex.Message);
            throw new InvalidOperationException($"Stripe error: {ex.Message}", ex);
        }
    }

    public async Task<bool> ConfirmPaymentAsync(string transactionId)
    {
        try
        {
            var options = new PaymentIntentConfirmOptions
            {
                PaymentMethod = "pm_card_visa",
            };

            var paymentIntent = await _paymentIntentService.ConfirmAsync(transactionId, options);

            _logger.LogInformation(
                "[REAL STRIPE] Confirmed PaymentIntent {PaymentIntentId}, status: {Status}", 
                paymentIntent.Id, paymentIntent.Status);

            return paymentIntent.Status == "succeeded";
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "[REAL STRIPE] Error confirming payment intent {TransactionId}", transactionId);
            throw new InvalidOperationException($"Stripe error: {ex.Message}", ex);
        }
    }

    public async Task<bool> RefundPaymentAsync(string transactionId)
    {
        try
        {
            var paymentIntent = await _paymentIntentService.GetAsync(transactionId);

            if (paymentIntent.Status != "succeeded")
            {
                throw new InvalidOperationException($"Cannot refund payment with status: {paymentIntent.Status}");
            }
            
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = transactionId,
            };

            var refund = await _refundService.CreateAsync(refundOptions);

            _logger.LogInformation(
                "[REAL STRIPE] Created refund {RefundId} for PaymentIntent {PaymentIntentId}, status: {Status}", 
                refund.Id, transactionId, refund.Status);

            return refund.Status == "succeeded";
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "[REAL STRIPE] Error refunding payment {TransactionId}", transactionId);
            throw new InvalidOperationException($"Stripe error: {ex.Message}", ex);
        }
    }
}
