using Loft.Common.Enums;

namespace ProductService.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ProductType Type { get; set; }
        public decimal Price { get; set; }
        public CurrencyType Currency { get; set; }
        public int Quantity { get; set; }
        public int ViewCount { get; set; } = 0;
        public ModerationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProductAttributeValue>? AttributeValues { get; set; }
        public ICollection<MediaFile>? MediaFiles { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
