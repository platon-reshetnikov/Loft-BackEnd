namespace Loft.Common.DTOs;

public record OrderItemDTO(long Id,long OrderId,long ProductId,int Quantity,decimal Price);