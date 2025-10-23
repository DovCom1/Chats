using Chats.Core.Enums;
using Chats.Core.Interfaces;
using Chats.Core.Models;
using Chats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chats.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetChatNameAsync(Guid chatId)
        {
            return await _context.Chats
                .Where(c => c.Id == chatId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<(Guid UserId, string Role)>> GetChatMembersAsync(Guid chatId)
        {
            return await _context.ChatMembers
                .Where(cm => cm.ChatId == chatId)
                .Select(cm => new ValueTuple<Guid, string>(cm.UserId, cm.Role))
                .ToListAsync();
        }

        public async Task<Guid?> GetAdminIdAsync(Guid chatId)
        {
            return await _context.ChatMembers
                .Where(cm => cm.ChatId == chatId && cm.Role == "Admin")
                .Select(cm => (Guid?)cm.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetAvatarUrlAsync(Guid chatId)
        {
            return await _context.Chats
                .Where(c => c.Id == chatId)
                .Select(c => c.AvatarUrl)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DoesMessageBelongToChatAsync(Guid chatId, Guid messageId)
        {
            return await _context.Messages
                .AnyAsync(m => m.Id == messageId && m.ChatId == chatId);
        }

        public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
        {
            return await _context.Chats
                .Include(c => c.Members)
                .Include(c => c.Messages) 
                .Where(c => c.Members.Any(m => m.UserId == userId))
                .OrderByDescending(c => c.Messages.Max(m => m.SentAt)) 
                .ToListAsync();
        }

        public async Task<ChatType?> GetChatTypeAsync(Guid chatId)
        {
            return await _context.Chats
                .Where(c => c.Id == chatId)
                .Select(c => (ChatType?)c.Type)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid?> FindPrivateChatAsync(Guid userA, Guid userB)
        {
            var chatId = await (
                from c in _context.Chats
                where c.Type == ChatType.Private
                join m1 in _context.ChatMembers on c.Id equals m1.ChatId
                join m2 in _context.ChatMembers on c.Id equals m2.ChatId
                where m1.UserId == userA && m2.UserId == userB
                select c.Id
            ).FirstOrDefaultAsync();

            return chatId == Guid.Empty ? null : chatId;
        }

        public async Task<Guid> CreatePrivateChatAsync(Guid userA, Guid userB)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Type = ChatType.Private,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chats.Add(chat);

            _context.ChatMembers.AddRange(
                new ChatMember { ChatId = chat.Id, UserId = userA, Role = "Member" },
                new ChatMember { ChatId = chat.Id, UserId = userB, Role = "Member" }
            );

            await _context.SaveChangesAsync();
            return chat.Id;
        }
    }
}
