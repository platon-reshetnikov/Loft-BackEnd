namespace UserService.Entities
{
    public class Chat
    {
        public long Id { get; set; }
        public long User1Id { get; set; }
        public long User2Id { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
