using Chats.Core.DTOs;

namespace Chats.Core.Interfaces
{
    public interface IMessagesService
    {
        Task<MessageDto> SendMessageAsync(Guid chatId, Guid senderId, string content);
        Task<ChatHistoryDto> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize);
        Task EditMessageAsync(Guid chatId, Guid messageId, string newContent, Guid userId);
        Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId);
        Task<bool> CanUserModifyMessageAsync(Guid messageId, Guid userId);
    }
}
