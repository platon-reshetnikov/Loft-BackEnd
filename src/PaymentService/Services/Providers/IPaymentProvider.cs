using Loft.Common.Enums;

namespace PaymentService.Services.Providers;

public interface IPaymentProvider
{
    PaymentMethod SupportedMethod { get; }
    Task<string> CreatePaymentAsync(decimal amount, long orderId);
    Task<bool> ConfirmPaymentAsync(string transactionId);
    Task<bool> RefundPaymentAsync(string transactionId);
}
