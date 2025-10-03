using Loft.Common.Enums;
using UserService.Entities;

namespace ProductService.Entities;

public class Product
{
    public long Id { get; set; }                 // Identity
    public long SellerId { get; set; }           // UserId
    public long CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Type { get; set; }            // physical/digital, nullable
    public string? Description { get; set; }     // nullable
    public decimal Price { get; set; }
    public string Currency { get; set; } = null!; // 'UAH','USD','EUR' и т.д.
    public string Status { get; set; } = "Pending"; // дефолт 'Pending'
    public int StockQuantity { get; set; } = 0;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}