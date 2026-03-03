namespace Loft.Common.DTOs
{ public record SendMessageRequest(long RecipientId, 
        string? MessageText, 
        string? FileUrl
);

    public record ChatMessageDTO
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsMod { get; set; } = false;
    }

    public record ChatDTO
    {
        public long ChatId { get; set; }
        public long User1Id { get; set; }
        public long User2Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChatMessageDTO? LastMessage { get; set; }
    }
}
