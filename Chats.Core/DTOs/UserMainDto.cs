namespace Chats.Core.DTOs
{
    public class UserMainDto
    {
        public Guid Id { get; set; }
        public string Uid { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string AvatarUrl { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
