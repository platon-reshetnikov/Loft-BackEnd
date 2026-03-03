using Loft.Common.Enums;

namespace OrderService.Entities;

public class OrderItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? ProductName { get; set; }
    public string? ImageUrl { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public ProductType ProductType { get; set; }
    public string? ProductAttributesJson { get; set; }
}
