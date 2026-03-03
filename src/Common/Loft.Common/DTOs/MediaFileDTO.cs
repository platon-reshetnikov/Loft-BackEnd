using Microsoft.AspNetCore.Http;

namespace Loft.Common.DTOs;

public class MyPublicMediaFileDTO
{
    public Guid Id { get; set; } 
    public string OriginalName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class UploadFileDto
{
    public IFormFile File { get; set; } = null!;
    public string Category { get; set; } = null!;
    public bool IsPrivate { get; set; } = false;
}
