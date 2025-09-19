namespace Loft.Common.DTOs;

public record SellerDTO(long Id,long UserId,string StoreName,string Description,decimal Balance,string StoreLogoUrl);