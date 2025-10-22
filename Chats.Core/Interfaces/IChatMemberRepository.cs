using Chats.Core.Models;

namespace Chats.Core.Interfaces
{
    public interface IChatMemberRepository
    {
        Task<IEnumerable<ChatMember>> GetMembersByChatIdAsync(Guid chatId);
        Task AddMemberAsync(ChatMember member);
        Task RemoveMemberAsync(Guid chatId, Guid userId);
        Task<bool> IsUserInChatAsync(Guid chatId, Guid userId);
        Task<string?> GetUserRoleAsync(Guid chatId, Guid userId);
    }
}
