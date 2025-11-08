using Chats.Core.DTOs;

namespace Chats.Core.Interfaces
{
    public interface IMessagesManager
    {
        Task<MessageDto> SendMessageAsync(Guid? chatId, SendMessageRequestDTO dto);
        Task EditMessageAsync(Guid chatId, Guid messageId, string newContent, Guid userId);
        Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId);
        Task<ChatHistoryDto> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize);
    }
}
