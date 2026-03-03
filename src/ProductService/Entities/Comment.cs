using Loft.Common.Enums;

namespace ProductService.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public ModerationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<MediaFile>? MediaFiles { get; set; }
    }
}
