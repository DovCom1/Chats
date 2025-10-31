using Chats.Core.Interfaces;
using Chats.Core.Models;
using Chats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chats.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Files)       
                .Include(m => m.Reactions)   
                .ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        {
            return await _context.Messages
                .Include(m => m.Files)
                .Include(m => m.Reactions)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task EditMessageAsync(Guid messageId, string newContent, Guid userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) throw new KeyNotFoundException("Message not found");
            if (message.SenderId != userId) throw new UnauthorizedAccessException("User cannot edit this message");

            message.Content = newContent;
            message.EditedAt = DateTime.UtcNow;

            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) throw new KeyNotFoundException("Message not found");
            if (message.SenderId != userId) throw new UnauthorizedAccessException("User cannot delete this message");

            message.Deleted = true;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanUserModifyMessageAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages
                .Where(m => m.Id == messageId)
                .Select(m => m.SenderId)
                .FirstOrDefaultAsync();

            return message == userId;
        }
    }
}
