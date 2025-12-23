using Chats.Core.DTOs;

namespace Chats.Core.Interfaces
{
    public interface IChatsManager
    {
        Task<ChatDto?> GetChatAsync(Guid chatId, Guid currentUserId);
        Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int pageNumber, int pageSize);
        Task AddUserToChatAsync(Guid chatId, Guid userId, string role, string? nickname = null);
        Task RemoveUserFromChatAsync(Guid chatId, Guid userId);
        Task<ChatDto> CreatePrivateChatAsync(Guid userA, Guid userB);
    }
}
