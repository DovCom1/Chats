using Chats.Core.DTOs;
using Chats.Core.Interfaces;
using Chats.Infrastructure.Services;

namespace Chats.Service.Managers
{
    public class MessagesManager : IMessagesManager
    {
        private readonly IMessagesService _messagesService;
        private readonly IChatsService _chatsService;
        private readonly INotifierChangerClient _notifierChangerClient; 
        private readonly UserServiceClient _userServiceClient;

        public MessagesManager(
            IMessagesService messagesService,
            IChatsService chatsService,
            INotifierChangerClient notifierChangerClient,
            UserServiceClient userServiceClient)
        {
            _messagesService = messagesService;
            _chatsService = chatsService;
            _notifierChangerClient = notifierChangerClient;
            _userServiceClient = userServiceClient;
        }

        public async Task<MessageDto> SendMessageAsync(Guid? chatId, SendMessageRequestDTO dto)
        {
            Guid actualChatId = chatId ?? Guid.Empty;
            Guid receiverId = dto.ReceiverId ?? Guid.Empty;

            if (actualChatId == Guid.Empty)
            {
                if (dto.ReceiverId is null)
                    throw new ArgumentException("ReceiverId is required when chatId is not specified.");

                receiverId = dto.ReceiverId.Value;
                actualChatId = await _chatsService.GetOrCreatePrivateChatAsync(dto.UserId, receiverId);
            }
            else
            {
                var membersDto = await _chatsService.GetChatMembersAsync(actualChatId);
                var otherMember = membersDto.Members.FirstOrDefault(m => m.UserId != dto.UserId);
                if (otherMember != null)
                {
                    receiverId = otherMember.UserId;
                }
            }

            var savedMessage = await _messagesService.SendMessageAsync(actualChatId, dto.UserId, dto.Content);

            _ = SendEventSafeAsync(actualChatId, dto.UserId, receiverId, dto.Content);

            return savedMessage;
        }

        private async Task SendEventSafeAsync(Guid chatId, Guid senderId, Guid receiverId, string content)
        {
            try
            {
                var senderInfo = await _userServiceClient.GetUserMainAsync(senderId);

                var receiverInfo = (receiverId != Guid.Empty)
                    ? await _userServiceClient.GetUserMainAsync(receiverId)
                    : null;

                var chatInfo = await _chatsService.GetChatAsync(chatId, senderId);

                var eventDto = new MessageEventDto
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    ChatId = chatId,
                    Message = content,
                    SenderName = senderInfo?.Nickname ?? "Unknown",
                    ReceiverName = receiverInfo?.Nickname ?? "Unknown",
                    ChatName = chatInfo?.Name,
                    CreatedAt = DateTime.UtcNow
                };

                await _notifierChangerClient.SendMessageEventAsync(eventDto);
            }
            catch
            {

            }
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
