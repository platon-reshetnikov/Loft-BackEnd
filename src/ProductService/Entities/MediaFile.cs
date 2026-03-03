using Loft.Common.Enums;

namespace ProductService.Entities
{
    public class MediaFile
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }
        public string Url { get; set; } = null!;
        public MediaTyp MediaTyp{ get; set; }
        public ModerationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
