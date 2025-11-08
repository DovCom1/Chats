using Chats.Core.DTOs;
using Chats.Core.Interfaces;

namespace Chats.Service.Managers
{
    public class MessagesManager : IMessagesManager
    {
        private readonly IMessagesService _messagesService;
        private readonly IChatsService _chatsService;

        public MessagesManager(IMessagesService messagesService, IChatsService chatsService)
        {
            _messagesService = messagesService;
            _chatsService = chatsService;
        }

        public async Task<MessageDto> SendMessageAsync(Guid? chatId, SendMessageRequestDTO dto)
        {
            Guid actualChatId = chatId ?? Guid.Empty;

            if (actualChatId == Guid.Empty)
            {
                if (dto.ReceiverId is null)
                    throw new ArgumentException("ReceiverId is required when chatId is not specified.");

                actualChatId = await _chatsService.GetOrCreatePrivateChatAsync(dto.UserId, dto.ReceiverId.Value);
            }

            return await _messagesService.SendMessageAsync(actualChatId, dto.UserId, dto.Content);
        }

        public async Task<ChatHistoryDto> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize) =>
            await _messagesService.GetChatHistoryAsync(chatId, pageNumber, pageSize);

        public async Task EditMessageAsync(Guid chatId, Guid messageId, string newContent, Guid userId) =>
            await _messagesService.EditMessageAsync(chatId, messageId, newContent, userId);

        public async Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId) =>
            await _messagesService.DeleteMessageAsync(chatId, messageId, userId);

        public async Task<bool> CanUserModifyMessageAsync(Guid messageId, Guid userId) =>
            await _messagesService.CanUserModifyMessageAsync(messageId, userId);
    }
}
