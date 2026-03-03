using Loft.Common.Enums;

namespace Loft.Common.DTOs;

public record OrderItemDTO(
    long Id,
    long OrderId,
    long ProductId,
    int Quantity,
    decimal Price,
    string? ProductName = null,
    string? ImageUrl = null,
    int? CategoryId = null,
    CategoryDto? Category = null,
    string? CategoryName = null,
    ProductType ProductType = ProductType.Physical,
    List<ProductAttributeValueDto>? AttributeValues = null
);