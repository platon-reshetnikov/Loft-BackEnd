namespace Loft.Common.DTOs;

public record CategoryDTO(long Id,string Name,long? ParentId,string CategoryImageUrl);