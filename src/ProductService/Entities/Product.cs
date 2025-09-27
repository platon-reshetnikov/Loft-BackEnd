using Loft.Common.Enums;
using UserService.Entities;

namespace ProductService.Entities;

public class Product
{
    public long Id { get; set; }
    public long UserId { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public long CategoryId { get; set; } 
    public string ProductImageUrl { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public User user { get; set; } 
}