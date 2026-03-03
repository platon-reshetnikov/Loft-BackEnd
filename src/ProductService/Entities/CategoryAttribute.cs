using Loft.Common.Enums;

namespace ProductService.Entities
{
    public class CategoryAttribute
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int AttributeId { get; set; }
        public AttributeEntity Attribute { get; set; } = null!;
        public bool IsRequired { get; set; } = false;
        public int OrderIndex { get; set; } = 0;
        public ModerationStatus Status { get; set; }
    }
    public class AttributeDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public AttributeType Type { get; set; }
        public string TypeDisplayName { get; set; } = null!;
        public string? OptionsJson { get; set; }
        public ModerationStatus? Status { get; set; }
    }
}
