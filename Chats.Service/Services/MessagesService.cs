using Chats.Core.DTOs;
using Chats.Core.Interfaces;
using Chats.Core.Models;

namespace Chats.Service.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;

        public MessagesService(IMessageRepository messageRepository, IChatRepository chatRepository)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
        }

        public async Task<MessageDto> SendMessageAsync(Guid chatId, Guid senderId, string content)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            var sent = await _messageRepository.SendMessageAsync(message);

            return new MessageDto
            {
                Id = sent.Id,
                SenderId = sent.SenderId,
                Content = sent.Content,
                SentAt = sent.SentAt,
                EditedAt = sent.EditedAt,
                Deleted = sent.Deleted
            };
        }

        public async Task<ChatHistoryDto> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize)
        {
            var messages = await _messageRepository.GetChatHistoryAsync(chatId, pageNumber, pageSize);
            return new ChatHistoryDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Messages = messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    EditedAt = m.EditedAt,
                    Deleted = m.Deleted
                })
            };
        }

        public async Task EditMessageAsync(Guid chatId, Guid messageId, string newContent, Guid userId)
        {
            if (!await _chatRepository.DoesMessageBelongToChatAsync(chatId, messageId))
                throw new InvalidOperationException("Message does not belong to this chat");

            await _messageRepository.EditMessageAsync(messageId, newContent, userId);
        }

        public async Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId)
        {
            if (!await _chatRepository.DoesMessageBelongToChatAsync(chatId, messageId))
                throw new InvalidOperationException("Message does not belong to this chat");

            await _messageRepository.DeleteMessageAsync(messageId, userId);
        }

        public async Task<bool> CanUserModifyMessageAsync(Guid messageId, Guid userId) =>
            await _messageRepository.CanUserModifyMessageAsync(messageId, userId);
    }
}
