using Chats.Core.DTOs;
using Chats.Core.Enums;
using Chats.Core.Interfaces;
using Chats.Infrastructure.Services;

namespace Chats.Service.Managers
{
    public class ChatsManager : IChatsManager
    {
        private readonly IChatsService _chatsService;
        private readonly UserServiceClient _userServiceClient;

        public ChatsManager(IChatsService chatsService, UserServiceClient userServiceClient)
        {
            _chatsService = chatsService;
            _userServiceClient = userServiceClient;
        }

        public async Task<ChatDto?> GetChatAsync(Guid chatId, Guid currentUserId)
        {
            var chat = await _chatsService.GetChatAsync(chatId, currentUserId);
            if (chat == null)
                return null;

            if (chat.Type == ChatType.Private)
            {
                var membersResponse = await _chatsService.GetChatMembersAsync(chatId);
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

        public async Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int pageNumber, int pageSize)
        {
            var chatList = await _chatsService.GetUserChatsAsync(userId, pageNumber, pageSize);

            foreach (var chat in chatList.Chats)
            {
                var chatDetails = await _chatsService.GetChatAsync(chat.Id, userId);
                if (chatDetails != null && chatDetails.Type == ChatType.Private)
                {
                    var membersResponse = await _chatsService.GetChatMembersAsync(chat.Id);
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
        public async Task<ChatDto> CreatePrivateChatAsync(Guid userA, Guid userB)
        {
            var chatId = await _chatsService.GetOrCreatePrivateChatAsync(userA, userB);

            var chat = await GetChatAsync(chatId, userA);
            if (chat == null)
                throw new InvalidOperationException("Chat creation failed");

            return chat;
        }

        public async Task<ChatMembersResponseDto> GetChatMembersAsync(Guid chatId) =>
            await _chatsService.GetChatMembersAsync(chatId);

        public async Task AddUserToChatAsync(Guid chatId, Guid userId, string role, string? nickname = null) =>
            await _chatsService.AddUserToChatAsync(chatId, userId, role, nickname);

        public async Task RemoveUserFromChatAsync(Guid chatId, Guid userId) =>
            await _chatsService.RemoveUserFromChatAsync(chatId, userId);
    }
}
