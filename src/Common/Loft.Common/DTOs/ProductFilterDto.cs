namespace Loft.Common.DTOs

{
    public class ProductFilterDto
    {
        public string? Search { get; set; } 
        public int? CategoryId { get; set; }
        public int? SellerId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<ProductAttributeFilterDto>? AttributeFilters { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

public class PagedResultFilterDto<T>
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; }
}
