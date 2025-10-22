using Chats.Core.DTOs;
using Chats.Core.Interfaces;
using Chats.Infrastructure.Services;

namespace Chats.Service.Managers
{
    public class ChatsManager : IChatsManager
    {
        private readonly IChatsService _chatService;
        private readonly UserServiceClient _userServiceClient;

        public ChatsManager(IChatsService chatService, UserServiceClient userServiceClient)
        {
            _chatService = chatService;
            _userServiceClient = userServiceClient;
        }


        public async Task<ChatDto?> GetChatAsync(Guid chatId, Guid currentUserId)
        {
            var chat = await _chatService.GetChatAsync(chatId, currentUserId);
            if (chat == null)
                return null;

            if (chat.Type == "private")
            {
                var membersResponse = await _chatService.GetChatMembersAsync(chatId);
                var otherMember = membersResponse.Members.FirstOrDefault(m => m.UserId != currentUserId);

                if (otherMember != null)
                {
                    var userInfo = await _userServiceClient.GetUserMainAsync(otherMember.UserId);
                    if (userInfo != null)
                    {
                        chat.Name = userInfo.Nickname;
                        chat.AvatarUrl = userInfo.AvatarUrl;
                    }
                    else
                    {
                        chat.Name = otherMember.Nickname ?? "Unknown";
                        chat.AvatarUrl = "/images/default-avatar.png";
                    }
                }
            }

            return chat;
        }

        public async Task<ChatMembersResponseDto> GetChatMembersAsync(Guid chatId)
        {
            return await _chatService.GetChatMembersAsync(chatId);
        }

        public async Task<MessageDto> SendMessageAsync(Guid chatId, Guid senderId, string content) =>
            await _chatService.SendMessageAsync(chatId, senderId, content);

        public async Task<ChatHistoryDto> GetChatHistoryAsync(Guid chatId, int pageNumber, int pageSize) =>
            await _chatService.GetChatHistoryAsync(chatId, pageNumber, pageSize);

        public async Task EditMessageAsync(Guid chatId, Guid messageId, string newContent, Guid userId) =>
            await _chatService.EditMessageAsync(chatId, messageId, newContent, userId);

        public async Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId) =>
            await _chatService.DeleteMessageAsync(chatId, messageId, userId);

        public async Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int pageNumber, int pageSize)
        {
            var chatList = await _chatService.GetUserChatsAsync(userId, pageNumber, pageSize);

            foreach (var chat in chatList.Chats)
            {
                var chatDetails = await _chatService.GetChatAsync(chat.Id, userId);
                if (chatDetails != null && chatDetails.Type == "private")
                {
                    var membersResponse = await _chatService.GetChatMembersAsync(chat.Id);
                    var otherMember = membersResponse.Members.FirstOrDefault(m => m.UserId != userId);
                    if (otherMember != null)
                    {
                        var userInfo = await _userServiceClient.GetUserMainAsync(otherMember.UserId);
                        if (userInfo != null)
                        {
                            chat.Name = userInfo.Nickname;
                            chat.AvatarUrl = userInfo.AvatarUrl;
                        }
                    }
                }
            }

            return chatList;
        }

        public async Task AddUserToChatAsync(Guid chatId, Guid userId, string role, string? nickname = null) =>
            await _chatService.AddUserToChatAsync(chatId, userId, role, nickname);

        public async Task RemoveUserFromChatAsync(Guid chatId, Guid userId) =>
            await _chatService.RemoveUserFromChatAsync(chatId, userId);

        public async Task<bool> CanUserModifyMessageAsync(Guid messageId, Guid userId) =>
            await _chatService.CanUserModifyMessageAsync(messageId, userId);
    }
}
