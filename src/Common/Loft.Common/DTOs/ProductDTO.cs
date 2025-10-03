using Loft.Common.Enums;

namespace Loft.Common.DTOs;

public record ProductDTO(
    long Id,                  
    long SellerId,
    long CategoryId,
    string Name, 
    string? Type,            
    string? Description,     
    decimal Price,
    string Currency,
    string? Status,           
    int StockQuantity,
    DateTime DateAdded,
    DateTime? UpdatedAt
);