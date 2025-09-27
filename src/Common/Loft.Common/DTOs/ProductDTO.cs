using Loft.Common.Enums;

namespace Loft.Common.DTOs;

public record ProductDTO(long Id,long UserId,string Name,decimal Price,int StockQuantity,long CategoryId,DeliveryType DeliveryType);
