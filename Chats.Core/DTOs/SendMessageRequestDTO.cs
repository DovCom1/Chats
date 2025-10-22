namespace Chats.Core.DTOs
{
    public class SendMessageRequestDTO
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = null!;
    }
}
