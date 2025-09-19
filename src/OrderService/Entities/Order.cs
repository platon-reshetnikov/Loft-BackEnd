using Loft.Common.Enums;
using PaymentService.Entities;
using ShippingAddressService.Entities;
using UserService.Entities;

namespace OrderService.Entities;

public class Order
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime UpdatedDate { get; set; }
    public User Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public Payment Payment { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
}