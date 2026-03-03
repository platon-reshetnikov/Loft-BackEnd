namespace ProductService.Entities

{
    public class ProductAttributeValue
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int AttributeId { get; set; }
        public AttributeEntity Attribute { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
