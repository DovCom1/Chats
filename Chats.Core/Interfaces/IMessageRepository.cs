using Chats.Core.Models;

namespace Chats.Core.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> SendMessageAsync(Message message);
        Task<IEnumerable<Message>> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task EditMessageAsync(Guid messageId, string newContent, Guid userId);
        Task DeleteMessageAsync(Guid messageId, Guid userId);
        Task<bool> CanUserModifyMessageAsync(Guid messageId, Guid userId);
    }
}
