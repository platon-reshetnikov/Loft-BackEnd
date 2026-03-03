using Loft.Common.DTOs;

namespace UserService.Services
{
    public interface IChatService
    {
        Task<ChatMessageDTO> SendMessage(long senderId, long recipientId, string? messageText, string? fileUrl = null);
        Task<List<ChatMessageDTO>> GetConversation(long userId, long otherUserId);
        Task MarkMessagesAsRead(long userId, long otherUserId);
        Task<List<ChatDTO>> GetUserChats(long userId);
        Task<ChatDTO?> GetChatById(long chatId);
        Task DeleteChat(long chatId);
    }
}
