using Chats.Core.DTOs;
using Chats.Core.Enums;
using Chats.Core.Interfaces;
using Chats.Core.Models;
using Chats.Infrastructure.Services;

namespace Chats.Application.Services
{
    public class ChatsService : IChatsService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatMemberRepository _chatMemberRepository;
        private readonly UserServiceClient _userServiceClient;

        public ChatsService(
            IChatRepository chatRepository,
            IChatMemberRepository chatMemberRepository,
            UserServiceClient userServiceClient)
        {
            _chatRepository = chatRepository;
            _chatMemberRepository = chatMemberRepository;
            _userServiceClient = userServiceClient;
        }

        public async Task<ChatDto?> GetChatAsync(Guid chatId, Guid currentUserId)
        {
            var chats = await _chatRepository.GetChatsByUserIdAsync(currentUserId);
            var chat = chats.FirstOrDefault(c => c.Id == chatId);
            if (chat == null) return null;

            var adminId = await _chatRepository.GetAdminIdAsync(chatId);
            string? chatName = chat.Name;
            string? avatarUrl = chat.AvatarUrl;

            if (chat.Type == ChatType.Private)
            {
                var members = chat.Members.Where(m => m.UserId != currentUserId).ToList();
                if (members.Any())
                {
                    var otherUser = members.First();
                    var userInfo = await _userServiceClient.GetUserMainAsync(otherUser.UserId);
                    if (userInfo != null)
                    {
                        chatName = userInfo.Nickname;
                        avatarUrl = userInfo.AvatarUrl;
                    }
                }
            }

            return new ChatDto
            {
                Id = chat.Id,
                Name = chatName,
                AdminId = adminId,
                AvatarUrl = avatarUrl,
                Type = chat.Type
            };
        }

        public async Task<ChatMembersResponseDto> GetChatMembersAsync(Guid chatId)
        {
            var members = await _chatMemberRepository.GetMembersByChatIdAsync(chatId);
            var dtoList = new List<ChatMemberDto>();

            foreach (var member in members)
            {
                var userInfo = await _userServiceClient.GetUserMainAsync(member.UserId);
                dtoList.Add(new ChatMemberDto
                {
                    UserId = member.UserId,
                    Role = member.Role,
                    Nickname = userInfo?.Nickname ?? member.Nickname ?? "Unknown",
                    AvatarUrl = userInfo?.AvatarUrl ?? "/images/default-avatar.png"
                });
            }

            return new ChatMembersResponseDto { Members = dtoList };
        }

        public async Task<Guid> GetOrCreatePrivateChatAsync(Guid senderId, Guid receiverId)
        {
            var existingChatId = await _chatRepository.FindPrivateChatAsync(senderId, receiverId);
            if (existingChatId.HasValue)
                return existingChatId.Value;

            return await _chatRepository.CreatePrivateChatAsync(senderId, receiverId);
        }

        public async Task AddUserToChatAsync(Guid chatId, Guid userId, string role, string? nickname = null)
        {
            var member = new ChatMember
            {
                ChatId = chatId,
                UserId = userId,
                Role = role,
                Nickname = nickname
            };
            await _chatMemberRepository.AddMemberAsync(member);
        }

        public async Task RemoveUserFromChatAsync(Guid chatId, Guid userId) =>
            await _chatMemberRepository.RemoveMemberAsync(chatId, userId);

        public async Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int pageNumber, int pageSize)
        {
            var allChats = await _chatRepository.GetChatsByUserIdAsync(userId);
            var chats = allChats
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ChatMainDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    AvatarUrl = c.AvatarUrl
                })
                .ToList();

            return new ChatListResponseDto { Chats = chats };
        }
    }
}
