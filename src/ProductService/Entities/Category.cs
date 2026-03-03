using Loft.Common.Enums;

namespace ProductService.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? SubCategories { get; set; }
        public ProductType Type { get; set; } = ProductType.Physical;
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; } = null!;
        public ModerationStatus Status { get; set; }
        public int ViewCount { get; set; } = 0;
        public ICollection<CategoryAttribute>? CategoryAttributes { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
