namespace Chats.Core.DTOs
{
    public class ChatMemberDto
    {
        public Guid UserId { get; set; }
        public string? Role { get; set; }
        public string? Nickname { get; set; }
        public string AvatarUrl { get; set; } = null!;
    }
}
