using Chats.Core.Interfaces;
using Chats.Core.Models;
using Chats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chats.Infrastructure.Repositories
{
    public class ChatMemberRepository : IChatMemberRepository
    {
        private readonly AppDbContext _context;

        public ChatMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatMember>> GetMembersByChatIdAsync(Guid chatId)
        {
            return await _context.ChatMembers
                .Where(cm => cm.ChatId == chatId)
                .ToListAsync();
        }

        public async Task AddMemberAsync(ChatMember member)
        {
            _context.ChatMembers.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(Guid chatId, Guid userId)
        {
            var member = await _context.ChatMembers
                .FirstOrDefaultAsync(cm => cm.ChatId == chatId && cm.UserId == userId);

            if (member == null)
                throw new KeyNotFoundException("Member not found");

            _context.ChatMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId)
        {
            return await _context.ChatMembers
                .AnyAsync(cm => cm.ChatId == chatId && cm.UserId == userId);
        }

        public async Task<string?> GetUserRoleAsync(Guid chatId, Guid userId)
        {
            return await _context.ChatMembers
                .Where(cm => cm.ChatId == chatId && cm.UserId == userId)
                .Select(cm => cm.Role)
                .FirstOrDefaultAsync();
        }
    }
}

