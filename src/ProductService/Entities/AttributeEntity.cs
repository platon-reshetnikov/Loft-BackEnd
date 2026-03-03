using Loft.Common.Enums;

namespace ProductService.Entities
{
    public class AttributeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; 
        public AttributeType Type { get; set; }
        public string TypeDisplayName { get; set; } = null!;
        public string? OptionsJson { get; set; }
        public ModerationStatus Status { get; set; } 
        public ICollection<CategoryAttribute>? CategoryAttributes { get; set; }
        public ICollection<ProductAttributeValue>? AttributeValues { get; set; }
    }
}
