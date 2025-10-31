namespace Chats.Core.DTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool Deleted { get; set; }
    }
}


