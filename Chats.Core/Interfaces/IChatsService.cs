using Chats.Core.DTOs;

namespace Chats.Core.Interfaces
{
    public interface IChatsService
    {
        Task<ChatDto?> GetChatAsync(Guid chatId, Guid userId);
        Task<ChatMembersResponseDto> GetChatMembersAsync(Guid chatId);
        Task<Guid> GetOrCreatePrivateChatAsync(Guid senderId, Guid receiverId);
        Task AddUserToChatAsync(Guid chatId, Guid userId, string role, string? nickname = null);
        Task RemoveUserFromChatAsync(Guid chatId, Guid userId);
        Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int pageNumber, int pageSize);
    }
}
