using Loft.Common.Enums;

namespace Loft.Common.DTOs;
public record CartItemDTO(
    long Id,
    long CartId,
    long ProductId,
    int Quantity,
    decimal Price = 0,
    string? ProductName = null,
    string? ImageUrl = null,
    int? CategoryId = null,
    CategoryDto? Category = null,
    string? CategoryName = null,
    ProductType ProductType = ProductType.Physical,
    List<ProductAttributeValueDto>? AttributeValues = null,
    DateTime? AddedAt = null
);