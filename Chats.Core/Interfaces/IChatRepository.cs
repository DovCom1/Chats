using Chats.Core.Enums;
using Chats.Core.Models;

namespace Chats.Core.Interfaces
{
    public interface IChatRepository
    {
        Task<string?> GetChatNameAsync(Guid chatId);
        Task<IEnumerable<(Guid UserId, string Role)>> GetChatMembersAsync(Guid chatId);
        Task<Guid?> GetAdminIdAsync(Guid chatId);
        Task<string?> GetAvatarUrlAsync(Guid chatId);
        Task<bool> DoesMessageBelongToChatAsync(Guid chatId, Guid messageId);
        Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
        Task<ChatType?> GetChatTypeAsync(Guid chatId);
        Task<Guid?> FindPrivateChatAsync(Guid userA, Guid userB);
        Task<Guid> CreatePrivateChatAsync(Guid userA, Guid userB);

    }
}
