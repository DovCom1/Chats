namespace Chats.Core.DTOs
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid? AdminId { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Type { get; set; }
    }

}
