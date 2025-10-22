using Chats.Core.DTOs;

namespace Chats.Core.Interfaces
{
    public interface IChatsManager
    {
        Task<ChatDto?> GetChatAsync(Guid chatId, Guid currentUserId);

        Task<ChatMembersResponseDto> GetChatMembersAsync(Guid chatId);

        Task<MessageDto> SendMessageAsync(Guid chatId, Guid senderId, string content);

        Task<ChatHistoryDto> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize);

        Task EditMessageAsync(Guid chatId, Guid messageId, string newContent, Guid userId);

        Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId);

        Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int pageNumber, int pageSize);

        Task AddUserToChatAsync(Guid chatId, Guid userId, string role, string? nickname = null);

        Task RemoveUserFromChatAsync(Guid chatId, Guid userId);

        Task<bool> CanUserModifyMessageAsync(Guid messageId, Guid userId);
    }
}
