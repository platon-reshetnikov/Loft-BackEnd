using ProductService.Entities;

namespace CategoryService.Entities;

public class Category
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long? ParentId { get; set; } 
    public string CategoryImageUrl { get; set; }
    public Category Parent { get; set; }
    public ICollection<Category> Subcategories { get; set; }
    public ICollection<Product> Products { get; set; } 
}